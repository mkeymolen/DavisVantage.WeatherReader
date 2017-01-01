namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpSettings : IDataLoggerSettings
    {
        public WeatherLinkIpSettings()
        {
            
        }
        public WeatherLinkIpSettings(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}
