using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.Logging;
using DavisVantage.WeatherReader.Models;
using Polly;

namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpDataLogger : IDataLogger<WeatherLinkIpSettings>, IDisposable
    {
        private TcpClient _tcpClient;
        private static readonly ILog s_logger = LogProvider.For<WeatherLinkIpDataLogger>();

        public bool Connect(WeatherLinkIpSettings settings)
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.ConnectAsync(settings.IpAddress, settings.Port).Wait();
                return _tcpClient.Connected;
            }
            catch (Exception)
            {
               s_logger.Error($"Could not connect to {settings.IpAddress}:{settings.Port}");
                return false;
            }
            
        }

        public bool WakeUp()
        {
            try
            {
                const byte NEWLINECHAR = 10;
                var networkStream = _tcpClient.GetStream();
                var dataAvailable = RetryPolicies.WakeUpPolicy.Execute(() =>
                {
                    s_logger.Info("Trying to wake up the console");
                    networkStream.WriteByte(NEWLINECHAR);
                    return networkStream.DataAvailable;
                });

                if (dataAvailable)
                {
                    s_logger.Info("Console has been woken up succesfully!");
                    return true;
                }

                s_logger.Warn("Could not initiate wake up call");
                return false;
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not initiate wake up call", ex);
                return false;
            }
        }

        public CurrentWeather ReadCurrentWeather()
        {
            try
            {
                const string COMMAND = "LOOP 1\n";
                var commandInBytes = Encoding.ASCII.GetBytes(COMMAND);

                var networkStream = _tcpClient.GetStream();
                networkStream.Write(commandInBytes, 0, commandInBytes.Length);
                if (networkStream.DataAvailable)
                {
                    StartAfterAckResponse(networkStream);                 
                    var dataBuffer = new byte[99];
                    networkStream.Read(dataBuffer, 0, dataBuffer.Length);

                    return ReadCurrentWeatherFromBuffer(dataBuffer);
                }
                s_logger.Warn("Could not read current weather data");
                return null;
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not read current weather data", ex);
                return null;
            }
        }

        private CurrentWeather ReadCurrentWeatherFromBuffer(byte[] dataBuffer)
        {
            var currentWeather = new CurrentWeather();
            currentWeather.Barometer = MetricConversion.InHgTohPa((float)BitConverter.ToInt16(dataBuffer, 7) / 1000);
            currentWeather.TempInside = MetricConversion.FahrenheitToDegrees((float)BitConverter.ToInt16(dataBuffer, 9) / 10);
            currentWeather.HumidityInside = Convert.ToInt32(dataBuffer[11]);
            currentWeather.TempOutside = MetricConversion.FahrenheitToDegrees((float)BitConverter.ToInt16(dataBuffer, 12) / 10);
            currentWeather.WindSpeed = MetricConversion.MphToKph(Convert.ToInt32(dataBuffer[14]));
            currentWeather.WindSpeed10Min = MetricConversion.MphToKph(Convert.ToInt32(dataBuffer[15]));
            currentWeather.WindDirectionDegrees = BitConverter.ToInt16(dataBuffer, 16);

            return currentWeather;
        }

        private void StartAfterAckResponse(NetworkStream networkStream)
        {
            const int ACK = 6;
            var ackFound = false;
            while (!ackFound)
            {
                var value = networkStream.ReadByte();
                ackFound = (value == ACK);
            }
        }

        public void ReadDayExtremes()
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

