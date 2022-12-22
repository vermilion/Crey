using System.Collections.Generic;
using System.Linq;

namespace Spear.Core.Message.Models
{
    public class DMessageInvoke<TDynamic> : DMessage
        where TDynamic : DMessageDynamic, new()
    {
        public virtual string ServiceId { get; set; }
        public virtual IDictionary<string, TDynamic> Parameters { get; set; }
        public virtual IDictionary<string, string> Headers { get; set; }

        public DMessageInvoke() { }

        public DMessageInvoke(InvokeMessage message)
        {
            SetValue(message);
        }

        public void SetValue(InvokeMessage message)
        {
            Id = message.Id;
            ServiceId = message.ServiceId;
            if (message.Parameters != null)
            {
                Parameters =
                    message.Parameters.ToDictionary(k => k.Key, v =>
                    {
                        var item = new TDynamic();
                        item.SetValue(v.Value);
                        return item;
                    });
            }

            if (message.Headers != null)
            {
                Headers = message.Headers.ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public InvokeMessage GetValue()
        {
            var message = new InvokeMessage
            {
                Id = Id,
                ServiceId = ServiceId
            };
            if (Parameters != null)
            {
                message.Parameters = Parameters.ToDictionary(k => k.Key, v => v.Value.GetValue());
            }

            if (Headers != null)
            {
                message.Headers = Headers.ToDictionary(k => k.Key, v => v.Value);
            }
            return message;
        }
    }
}
