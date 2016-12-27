using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using DavisVantage.WeatherReader.Logging;
using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

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

        public CurrentWeather ReadCurrentWeather(bool valueInMetric)
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

                    return GetCurrentWeatherFromBytes(dataBuffer, valueInMetric);
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

        public WeatherExtremes ReadWeatherExtremes(bool valueInMetric)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_tcpClient?.Connected ?? false)
            {
#if NET451
                _tcpClient.Close();
                #else
   _tcpClient.Dispose();
#endif  
            }
            _consoleWokenUp = false;
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
        private CurrentWeather GetCurrentWeatherFromBytes(byte[] dataBuffer, bool valueInMetric)
        {
            var currentWeather = new CurrentWeather();
            currentWeather.UtcTime = DateTime.UtcNow;
            currentWeather.ConsoleInfo = new ConsoleInfo()
            {
                IssBatteryStatus = Convert.ToInt32(dataBuffer[86]),
                ConsoleBatteryVoltage = Math.Round(Convert.ToDecimal((float)BitConverter.ToInt16(dataBuffer, 87)*300/512/100.0),1)
            };
            var barometerFromDataBuffer = (float) BitConverter.ToInt16(dataBuffer, 7) / 1000;
            currentWeather.Barometer = valueInMetric ? MetricConversion.InHgTohPa(barometerFromDataBuffer) : Convert.ToInt32(barometerFromDataBuffer);
            var tempInsideFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, 9) / 10;
            currentWeather.TempInside = valueInMetric ? MetricConversion.FahrenheitToDegrees(tempInsideFromDataBuffer) : Convert.ToDecimal(tempInsideFromDataBuffer);
            currentWeather.HumidityInside = Convert.ToInt32(dataBuffer[11]);
            var tempOutsideFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, 12) / 10;
            currentWeather.TempOutside = valueInMetric ? MetricConversion.FahrenheitToDegrees(tempOutsideFromDataBuffer) : Convert.ToDecimal(tempOutsideFromDataBuffer);
            var windSpeedFromDataBuffer = Convert.ToInt32(dataBuffer[14]);
            currentWeather.WindSpeed = valueInMetric ? MetricConversion.MphToKph(windSpeedFromDataBuffer) : windSpeedFromDataBuffer;
            windSpeedFromDataBuffer = Convert.ToInt32(dataBuffer[15]);
            currentWeather.WindSpeed10Min = valueInMetric ? MetricConversion.MphToKph(windSpeedFromDataBuffer) : windSpeedFromDataBuffer;
            currentWeather.WindDirectionDegrees = BitConverter.ToInt16(dataBuffer, 16);
            currentWeather.ExtraTemperatures = GetExtraTemperaturesFromBuffer(dataBuffer, 18, 7, valueInMetric);
            currentWeather.SoilTemperatures = GetExtraTemperaturesFromBuffer(dataBuffer, 25, 4, valueInMetric);
            currentWeather.LeafTemperatures = GetExtraTemperaturesFromBuffer(dataBuffer, 29, 4, valueInMetric);
            currentWeather.HumidityOutside = Convert.ToInt32(dataBuffer[33]);
            currentWeather.ExtraHumidities = GetListValuesFromBuffer(dataBuffer,34,7);
            var rainRateTicks = (float)BitConverter.ToInt16(dataBuffer, 41);
            currentWeather.RainRate = valueInMetric ? Convert.ToDecimal(rainRateTicks / 5) : Convert.ToDecimal(rainRateTicks/100);
            currentWeather.UvIndex = Convert.ToInt32(dataBuffer[43]);
            currentWeather.SolarRadiation = BitConverter.ToInt16(dataBuffer, 44);
            var stormRainTicks = (float)BitConverter.ToInt16(dataBuffer, 46);
            currentWeather.StormRain = valueInMetric ? Convert.ToDecimal(stormRainTicks / 5) : Convert.ToDecimal(stormRainTicks / 100);
            var rainToday = (float)BitConverter.ToInt16(dataBuffer, 50);
            currentWeather.RainToday = valueInMetric ? Convert.ToDecimal(rainToday / 5) : Convert.ToDecimal(rainToday / 100);
            currentWeather.SoilMoistures = GetListValuesFromBuffer(dataBuffer, 62, 4);
            currentWeather.LeafWetnesses = GetListValuesFromBuffer(dataBuffer, 66, 4);
            var sunriseValueFromDataBuffer = (float) BitConverter.ToInt16(dataBuffer, 91);
            currentWeather.SunRise = currentWeather.UtcTime.Date.AddHours(Math.Floor(sunriseValueFromDataBuffer/100)).AddMinutes(sunriseValueFromDataBuffer%100);
            var sunsetValueFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, 93);
            currentWeather.SunSet = currentWeather.UtcTime.Date.AddHours(Math.Floor(sunsetValueFromDataBuffer / 100)).AddMinutes(sunsetValueFromDataBuffer % 100);
            return currentWeather;
        }

        private List<decimal> GetExtraTemperaturesFromBuffer(byte[] dataBuffer, int byteOffset, int byteLength, bool valueInMetric)
        {
            var extraTemperatures = new List<decimal>();
            for (var i = byteOffset; i < byteOffset + byteLength; i++)
            {
                var byteValue = Convert.ToInt32(dataBuffer[i]);
                // ignore max byte values --> no sensor
                if (byteValue != byte.MaxValue)
                {
                    var temperature = byteValue - 90;
                    extraTemperatures.Add(valueInMetric ? MetricConversion.FahrenheitToDegrees(temperature) : temperature);
                }
            }
            return extraTemperatures;
        }
        private List<int> GetListValuesFromBuffer(byte[] dataBuffer, int byteOffset, int byteLength)
        {
            var listValues = new List<int>();
            for (var i = byteOffset; i < byteOffset+ byteLength; i++)
            {
                var byteValue = Convert.ToInt32(dataBuffer[i]);
                // ignore max byte values --> no sensor
                if (byteValue != byte.MaxValue)
                {
                    listValues.Add(byteValue);
                }
            }
            return listValues;
        }
    }
}

