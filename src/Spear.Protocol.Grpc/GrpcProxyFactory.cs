using System;
using System.Linq;
using Grpc.Core;
using Grpc.Net.Client;
using Spear.Core;
using Spear.Core.Config;
using Spear.Core.Exceptions;
using Spear.Core.Micro.Services;
using Spear.ProxyGenerator;
using Spear.ProxyGenerator.Impl;
using Spear.ProxyGenerator.Proxy;

namespace Spear.Protocol.Grpc
{
    public class GrpcProxyFactory : ProxyFactory
    {
        public GrpcProxyFactory(IResolver resolver, IProxyProvider proxyProvider, AsyncProxyGenerator proxyGenerator)
            : base(resolver, proxyProvider, proxyGenerator)
        {
            //GRpc Http支持
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
    }
}
