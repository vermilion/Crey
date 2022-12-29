using System.Reflection;
using System.Reflection.Emit;

namespace Crey.Proxy.ProxyGenerator;

internal sealed class PropertyAccessorInfo
{
    public MethodInfo InterfaceGetMethod { get; }
    public MethodInfo InterfaceSetMethod { get; }
    public MethodBuilder GetMethodBuilder { get; set; }
    public MethodBuilder SetMethodBuilder { get; set; }

    public PropertyAccessorInfo(MethodInfo interfaceGetMethod, MethodInfo interfaceSetMethod)
    {
        InterfaceGetMethod = interfaceGetMethod;
        InterfaceSetMethod = interfaceSetMethod;
    }
}
