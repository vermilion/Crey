namespace Spear.Core.ServiceDiscovery
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
}
