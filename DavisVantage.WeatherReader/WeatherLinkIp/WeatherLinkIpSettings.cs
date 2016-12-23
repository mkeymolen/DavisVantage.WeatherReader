namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpSettings : IDataLoggerSettings
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}
