using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using DavisVantage.WeatherReader.Logging;
using DavisVantage.WeatherReader.Models;

namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpDataLogger : IDataLogger<WeatherLinkIpSettings>, IDisposable
    {
        private TcpClient _tcpClient;
        private static readonly ILog s_logger = LogProvider.For<WeatherLinkIpDataLogger>();
        private bool _consoleWokenUp;

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

        public void WakeUp()
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
                    _consoleWokenUp = true;
                    s_logger.Info("Console has been woken up succesfully!");
                }
                else
                {
                    s_logger.Warn("Could not initiate wake up call. Response from Console was empty.");
                }
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not initiate wake up call", ex);
            }
        }

        public CurrentWeather ReadCurrentWeather()
        {
            try
            {
                if (!_consoleWokenUp)
                {
                    WakeUp();
                }
                const string COMMAND = "LOOP 1\n";
                var commandInBytes = Encoding.ASCII.GetBytes(COMMAND);

                var networkStream = _tcpClient.GetStream();
                networkStream.Write(commandInBytes, 0, commandInBytes.Length);
                if (networkStream.DataAvailable)
                {
                    ReadUntilAckByte(networkStream);                 
                    var dataBuffer = new byte[99];
                    networkStream.Read(dataBuffer, 0, dataBuffer.Length);

                    return GetCurrentWeatherFromBytes(dataBuffer);
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

        private CurrentWeather GetCurrentWeatherFromBytes(byte[] dataBuffer)
        {
            var currentWeather = new CurrentWeather
            {
                Barometer = MetricConversion.InHgTohPa((float) BitConverter.ToInt16(dataBuffer, 7) / 1000),
                TempInside = MetricConversion.FahrenheitToDegrees((float) BitConverter.ToInt16(dataBuffer, 9) / 10),
                HumidityInside = Convert.ToInt32(dataBuffer[11]),
                TempOutside = MetricConversion.FahrenheitToDegrees((float) BitConverter.ToInt16(dataBuffer, 12) / 10),
                WindSpeed = MetricConversion.MphToKph(Convert.ToInt32(dataBuffer[14])),
                WindSpeed10Min = MetricConversion.MphToKph(Convert.ToInt32(dataBuffer[15])),
                WindDirectionDegrees = BitConverter.ToInt16(dataBuffer, 16),
                ExtraTemperatures = GetExtraTemperaturesFromBuffer(dataBuffer, 18,7),
                SoilTemperatures = GetExtraTemperaturesFromBuffer(dataBuffer,25,4)
            };

            return currentWeather;
        }

        private List<decimal> GetExtraTemperaturesFromBuffer(byte[] dataBuffer, int byteOffset, int byteLength)
        {
            var extraTemperatures = new List<decimal>();
            for (var i = byteOffset; i < byteOffset + byteLength; i++)
            {
                var byteValue = Convert.ToInt32(dataBuffer[i]);
                // ignore max byte values --> no sensor
                if (byteValue != byte.MaxValue)
                {
                    extraTemperatures.Add(MetricConversion.FahrenheitToDegrees(byteValue-90));
                }
            }
            return extraTemperatures;
        }

        private void ReadUntilAckByte(NetworkStream networkStream)
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
            _consoleWokenUp = false;
        }
    }
}

