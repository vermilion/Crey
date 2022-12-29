using System.Reflection;

namespace Crey.Proxy.ProxyGenerator;

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
