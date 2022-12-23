using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Spear.Core.Extensions;

namespace Spear.Core.Micro.Services
{
    public class ServiceAddress
    {
        /// <summary> 服务协议 </summary>
        public string Host { get; set; }

        private int _port;

        /// <summary> 端口号 </summary>
        public int Port
        {
            get => _port > 80 ? _port : 5000;
            set => _port = value;
        }

        /// <summary> 权重 </summary>
        public double Weight { get; set; } = 1;

        public ServiceAddress() { }

        public ServiceAddress(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }

    /// <summary> 服务地址扩展 </summary>
    public static class ServiceAddressExtensions
    {
        public static string ToJson(this ServiceAddress address)
        {
            return JsonSerializer.Serialize(address);
        }

        /// <summary>
        /// 获取线程级随机数
        /// </summary>
        /// <returns></returns>
        private static Random Random()
        {
            var bytes = new byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            var seed = BitConverter.ToInt32(bytes, 0);
            var tick = DateTime.Now.Ticks + (seed);
            return new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        /// <summary> 权重随机 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ServiceAddress Random(this IList<ServiceAddress> services)
        {
            if (services == null || !services.Any()) return null;
            if (services.Count == 1) return services.First();

            //权重随机
            var sum = services.Sum(t => t.Weight);
            var rand = Random().NextDouble() * sum;
            var tempWeight = 0D;
            foreach (var service in services)
            {
                tempWeight += service.Weight;
                if (rand <= tempWeight)
                    return service;
            }

            return services.RandomSort().First();
        }
    }
}
