using Grpc.Core;
using Spear.Core.Message;

namespace Spear.Protocol.Grpc
{
    internal class MethodDefinitionGenerator
    {
        public static Method<TRequest, TResponse> CreateMethodDefinition<TRequest, TResponse>(MethodType methodType, string serviceName, string methodName, IMessageCodec codec)
            where TRequest : class
            where TResponse : class
        {
            var method = new Method<TRequest, TResponse>(
                type: methodType,
                serviceName: serviceName,
                name: methodName,
                requestMarshaller: Marshallers.Create(
                    serializer: request => codec.Encode(request),
                    deserializer: bytes => codec.Decode<TRequest>(bytes)),
                responseMarshaller: Marshallers.Create(
                    serializer: response => codec.Encode(response),
                    deserializer: bytes => codec.Decode<TResponse>(bytes))
            );

            return method;
        }
    }
}
