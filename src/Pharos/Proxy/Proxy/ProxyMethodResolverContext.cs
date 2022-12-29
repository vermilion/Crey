using System.Reflection;

namespace Psi.Proxy.Proxy;

internal class ProxyMethodResolverContext
{
    public PackedArgs Packed { get; }
    public MethodBase Method { get; }

    public ProxyMethodResolverContext(PackedArgs packed, MethodBase method)
    {
        Packed = packed;
        Method = method;
    }
}
