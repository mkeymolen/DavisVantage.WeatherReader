using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.Logging;
using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpDataLogger : IDataLogger<WeatherLinkIpSettings>
    {

        public WeatherLinkIpSettings Settings { get; set; }
        private readonly IByteReader _byteReader;
        private TcpClient _tcpClient;
        private static readonly ILog s_logger = LogProvider.For<WeatherLinkIpDataLogger>();

        public WeatherLinkIpDataLogger(IByteReader byteReader, IDataLoggerSettings settings)
        {
            _byteReader = byteReader;
            Settings = settings as WeatherLinkIpSettings;
        }

        public bool Connect()
        {
            try
            {
                if (Settings == null)
                {
                    s_logger.Warn("No connection could be established. Settings are empty.");
                    return false;
                }
                _tcpClient = new TcpClient();
                // wait for connection
                _tcpClient.ConnectAsync(Settings.IpAddress, Settings.Port).Wait();
                return _tcpClient.Connected;
            }
            catch (Exception)
            {
               s_logger.Error($"Could not connect to {Settings?.IpAddress}:{Settings?.Port}");
                return false;
            }
            
        }

        public async Task<CurrentWeather> ReadCurrentWeather(bool valuesInMetric)
        {
            try
            {
                if (WakeUp())
                {
                    const string COMMAND = "LOOP 1\n";
                    var commandInBytes = Encoding.ASCII.GetBytes(COMMAND);

                    var networkStream = _tcpClient.GetStream();
                    networkStream.ReadTimeout = 10000;
                    networkStream.Write(commandInBytes, 0, commandInBytes.Length);
                    if (networkStream.DataAvailable)
                    {
                        ReadUntilAckByte(networkStream);
                        var dataBuffer = new byte[99];
                        networkStream.Read(dataBuffer, 0, dataBuffer.Length);

                        return await _byteReader.ReadCurrentWeatherFromByteArray(dataBuffer, valuesInMetric);
                    }
                    s_logger.Warn("Could not read current weather data. No data available");
                }

                return null;

            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not read current weather data. ", ex);
                return null;
            }
        }

        public async Task<WeatherExtremes> ReadWeatherExtremes(bool valuesInMetric)
        {
            try
            {
                if (WakeUp())
                {
                    const string COMMAND = "HILOWS\n";
                    var commandInBytes = Encoding.ASCII.GetBytes(COMMAND);

                    var networkStream = _tcpClient.GetStream();
                    networkStream.Write(commandInBytes, 0, commandInBytes.Length);
                    if (networkStream.DataAvailable)
                    {
                        ReadUntilAckByte(networkStream);
                        var dataBuffer = new byte[438];
                        networkStream.Read(dataBuffer, 0, dataBuffer.Length);

                        return await _byteReader.ReadWeatherExtremesFromByteArray(dataBuffer, valuesInMetric);
                    }
                    s_logger.Warn("Could not read weather extremes. No data available");
                }
                
                return null;
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not read weather extremes.", ex);
                return null;
            }
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
        }

        private bool WakeUp()
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
                else
                {
                    s_logger.Warn("Could not initiate wake up call. Response from Console was empty.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not initiate wake up call. ", ex);
                return false;
            }
        }

        private void ReadUntilAckByte(NetworkStream networkStream)
        {
            const int ACK = 6;
            var ackFound = false;
            while (!ackFound)
            {
                var value = networkStream.ReadByte();
                if (value == -1)
                {
                    throw new Exception("Empty response from console");
                }
                ackFound = (value == ACK);
            }
        }
    }
}

