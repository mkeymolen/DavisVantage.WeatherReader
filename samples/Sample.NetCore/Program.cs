﻿using System;
using DavisVantage.WeatherReader.WeatherLinkIp;
using NLog;

namespace DavisVantage.WeatherReader.Sample
{
    public class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            var dataloggerSettings = new WeatherLinkIpSettings("192.168.1.140", 22222);
            s_logger.Info("Started");
            using (var datalogger = new WeatherLinkIpDataLogger(new WeatherLinkIpByteReader(), dataloggerSettings))
            {
                if (datalogger.Connect())
                {
                    var currentWeather = datalogger.ReadCurrentWeather(true);
                    s_logger.Info(currentWeather.Result);
                    var weatherExtremes = datalogger.ReadWeatherExtremes(true);
                    s_logger.Info(weatherExtremes.Result);
                }
                else
                {
                    s_logger.Warn("Not connected to datalogger");
                }
            }
            Console.ReadKey();
            s_logger.Info("Exit");
            // read currentweatherdata
            // read extremes
            // weatherstation values
        }
    }
}
