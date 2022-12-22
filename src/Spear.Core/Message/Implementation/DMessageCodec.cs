﻿using Spear.Core.Config;
using Spear.Core.Extensions;
using Spear.Core.Message.Models;
using System;
using System.Threading.Tasks;

namespace Spear.Core.Message.Implementation
{
    public abstract class DMessageCodec<TDynamic, TInvoke, TResult> : IMessageCodec
        where TDynamic : DMessageDynamic, new()
        where TInvoke : DMessageInvoke<TDynamic>, new()
        where TResult : DMessageResult<TDynamic>, new()
    {
        private readonly IMessageSerializer _serializer;
        private readonly SpearConfig _config;

        protected DMessageCodec(IMessageSerializer serializer, SpearConfig config)
        {
            _serializer = serializer;
            _config = config ?? new SpearConfig();
        }

        protected virtual byte[] OnEncode(object message)
        {
            if (message == null) return new byte[0];

            if (message.GetType() == typeof(byte[]))
                return (byte[])message;

            if (message is InvokeMessage invoke)
            {
                //var model = Activator.CreateInstance(typeof(TInvoke),invoke);
                var model = new TInvoke();
                model.SetValue(invoke);
                return _serializer.Serialize(model);
            }

            if (message is MessageResult result)
            {
                var model = new TResult();
                model.SetResult(result);
                return _serializer.Serialize(model);
            }

            return _serializer.SerializeNoType(message);
        }

        protected virtual object OnDecode(byte[] data, Type type)
        {
            if (data == null || data.Length == 0)
                return null;
            if (type == typeof(InvokeMessage))
            {
                var item = _serializer.Deserialize<TInvoke>(data);
                return item.GetValue();
            }

            if (type == typeof(MessageResult))
            {
                var item = _serializer.Deserialize<TResult>(data);
                return item.GetValue();
            }

            return _serializer.DeserializeNoType(data, type);
        }

        public async Task<byte[]> EncodeAsync(object message)
        {
            if (message == null) return new byte[0];
            var buffer = OnEncode(message);
            return buffer;
        }

        public async Task<object> DecodeAsync(byte[] data, Type type)
        {
            var obj = OnDecode(data, type);
            var result = obj.CastTo(type);
            return result;
        }
    }
}
