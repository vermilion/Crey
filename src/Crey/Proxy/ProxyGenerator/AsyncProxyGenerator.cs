using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Crey.Proxy.Abstractions;

namespace Crey.Proxy.ProxyGenerator;

/// <summary>
/// Reflection-based proxy generator
/// </summary>
public class AsyncProxyGenerator : IDisposable
{
    private readonly ConcurrentDictionary<Type, Dictionary<Type, Type>> _proxyTypeCaches = new();
    private readonly ProxyAssembly _proxyAssembly = new();

    private readonly MethodInfo _dispatchProxyInvokeMethod = typeof(ProxyExecutor).GetTypeInfo().GetDeclaredMethod(nameof(ProxyExecutor.Invoke));
    private readonly MethodInfo _dispatchProxyInvokeAsyncMethod = typeof(ProxyExecutor).GetTypeInfo().GetDeclaredMethod(nameof(ProxyExecutor.InvokeAsync));
    private readonly MethodInfo _dispatchProxyInvokeAsyncTMethod = typeof(ProxyExecutor).GetTypeInfo().GetDeclaredMethod(nameof(ProxyExecutor.InvokeAsyncT));

    public object CreateProxy(Type interfaceType, Type baseType, IProxyProvider proxyProvider)
    {
        var proxiedType = GetProxyType(baseType, interfaceType);
        return Activator.CreateInstance(proxiedType, proxyProvider, null, new ProxyHandler(this));
    }

    private Type GetProxyType(Type baseType, Type interfaceType)
    {
        if (!_proxyTypeCaches.TryGetValue(baseType, out var interfaceToProxy))
        {
            interfaceToProxy = new Dictionary<Type, Type>();
            _proxyTypeCaches[baseType] = interfaceToProxy;
        }

        if (!interfaceToProxy.TryGetValue(interfaceType, out var generatedProxy))
        {
            generatedProxy = GenerateProxyType(baseType, interfaceType);
            interfaceToProxy[interfaceType] = generatedProxy;
        }

        return generatedProxy;
    }

    private Type GenerateProxyType(Type baseType, Type interfaceType)
    {
        var baseTypeInfo = baseType.GetTypeInfo();
        if (!interfaceType.GetTypeInfo().IsInterface)
        {
            throw new ArgumentException($"InterfaceType_Must_Be_Interface, {interfaceType.FullName}", nameof(interfaceType));
        }

        if (baseTypeInfo.IsSealed)
        {
            throw new ArgumentException($"BaseType_Cannot_Be_Sealed, {baseTypeInfo.FullName}", nameof(baseType));
        }

        if (baseTypeInfo.IsAbstract)
        {
            throw new ArgumentException($"BaseType_Cannot_Be_Abstract {baseType.FullName}", nameof(baseType));
        }

        var pb = _proxyAssembly.CreateProxy("Proxy", baseType);

        foreach (var t in interfaceType.GetTypeInfo().ImplementedInterfaces)
            pb.AddInterfaceImpl(t);

        pb.AddInterfaceImpl(interfaceType);

        var generatedProxyType = pb.CreateType();
        return generatedProxyType;
    }

    private ProxyMethodResolverContext Resolve(object[] args)
    {
        var packed = new PackedArgs(args);
        var method = _proxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
        if (method.IsGenericMethodDefinition)
            method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);

        return new ProxyMethodResolverContext(packed, method);
    }

    public object Invoke(object[] args)
    {
        var context = Resolve(args);

        // Call (protected method) DispatchProxyAsync.Invoke()
        object returnValue = null;
        try
        {
            Debug.Assert(_dispatchProxyInvokeMethod != null);
            returnValue = _dispatchProxyInvokeMethod.Invoke(context.Packed.DispatchProxy,
                new object[] { context.Method, context.Packed.Args });
            context.Packed.ReturnValue = returnValue;
        }
        catch (TargetInvocationException tie)
        {
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
        }

        return returnValue;
    }

    public async Task InvokeAsync(object[] args)
    {
        var context = Resolve(args);

        // Call (protected Task method) NetCoreStackDispatchProxy.InvokeAsync()
        try
        {
            Debug.Assert(_dispatchProxyInvokeAsyncMethod != null);
            await (Task)_dispatchProxyInvokeAsyncMethod.Invoke(context.Packed.DispatchProxy,
                                                                   new object[] { context.Method, context.Packed.Args });
        }
        catch (TargetInvocationException tie)
        {
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
        }
    }

    public async Task<T> InvokeAsync<T>(object[] args)
    {
        var context = Resolve(args);

        var returnValue = default(T);
        try
        {
            Debug.Assert(_dispatchProxyInvokeAsyncTMethod != null);
            var genericmethod = _dispatchProxyInvokeAsyncTMethod.MakeGenericMethod(typeof(T));
            returnValue = await (Task<T>)genericmethod.Invoke(context.Packed.DispatchProxy,
                                                                   new object[] { context.Method, context.Packed.Args });
            context.Packed.ReturnValue = returnValue;
        }
        catch (TargetInvocationException tie)
        {
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
        }
        return returnValue;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
