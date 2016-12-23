namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpSettings : IDataLoggerSettings
    {
        public WeatherLinkIpSettings(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
        public string IpAddress { get;}
        public int Port { get;}
    }
}
