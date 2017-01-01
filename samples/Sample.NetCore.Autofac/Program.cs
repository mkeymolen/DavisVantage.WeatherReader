using System;
using Autofac;
using DavisVantage.WeatherReader;
using DavisVantage.WeatherReader.WeatherLinkIp;
using NLog;

namespace Sample.NetCore.Autofac
{
    public class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            var container = AutofacSetup();
            using (var scope = container.BeginLifetimeScope())
            {
                using (var dataLogger = scope.Resolve<IDataLogger<WeatherLinkIpSettings>>())
                {
                    if (dataLogger.Connect())
                    {
                        var currentWeather = dataLogger.ReadCurrentWeather(true);
                        s_logger.Info(currentWeather);
                        var weatherExtremes = dataLogger.ReadWeatherExtremes(true);
                        s_logger.Info(weatherExtremes);
                    }
                    else
                    {
                        s_logger.Warn("Not connected to datalogger");
                    }
                }
            }
            Console.ReadLine();
        }

        private static IContainer AutofacSetup()
        {
            var weatherlinkIpSettings = ConfigHelper.Read<WeatherLinkIpSettings>("WeatherLinkIpSettings");
            var builder = new ContainerBuilder();

            builder.Register(c => weatherlinkIpSettings).As<IDataLoggerSettings>();
            builder.RegisterType<WeatherLinkIpByteReader>().As<IByteReader>();
            builder.RegisterType<WeatherLinkIpDataLogger>().As<IDataLogger<WeatherLinkIpSettings>>();
            var container = builder.Build();
            return container;
        }
    }
}
