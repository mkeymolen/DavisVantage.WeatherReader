using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.WeatherLinkIp;

namespace DavisVantage.WeatherReader.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dataloggerSettings = new WeatherLinkIpSettings("192.168.1.140",22222);
            using (var datalogger = new WeatherLinkIpDataLogger())
            {
                datalogger.Connect(dataloggerSettings);
                datalogger.WakeUp();
            } 
            // read currentweatherdata
            // read extremes
            // weatherstation values
        }
    }
}
