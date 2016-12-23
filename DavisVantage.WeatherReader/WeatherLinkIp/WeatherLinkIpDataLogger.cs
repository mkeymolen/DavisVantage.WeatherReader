using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.Logging;

namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpDataLogger : IDataLogger<WeatherLinkIpSettings>, IDisposable
    {
        private TcpClient _tcpClient;
        private static readonly ILog s_logger = LogProvider.For<WeatherLinkIpDataLogger>();

        public void Connect(WeatherLinkIpSettings settings)
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.ConnectAsync(settings.IpAddress, settings.Port);
            }
            catch (Exception)
            {
               s_logger.Error($"Could not connect to {settings.IpAddress}:{settings.Port}");
            }
            
        }

        public void WakeUp()
        {
            try
            {
                const byte NEWLINECHAR = 10;
                var networkStream = _tcpClient.GetStream();
                networkStream.WriteByte(NEWLINECHAR);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public Task ReadCurrentWeather()
        {
            throw new NotImplementedException();
        }

        public Task ReadDayExtremes()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_tcpClient?.Connected ?? false)
            {
                _tcpClient.Dispose();
            }
        }
    }
}
