using System;
using DavisVantage.WeatherReader.Extensions;

namespace DavisVantage.WeatherReader.Models.Extremes
{
    public class WeatherdayExtremes
    {
        public int BarometerMax { get; set; }
        public int BarometerMin { get; set; }
        public DateTime BarometerMaxTime { get; set; }
        public DateTime BarometerMinTime { get; set; }
        public int MaxWindSpeed { get; set; }
        public DateTime MaxWindSpeedTime { get; set; }
        public decimal MaxIndoorTemp { get; set; }
        public DateTime MaxIndoorTempTime { get; set; }
        public decimal MinIndoorTemp { get; set; }
        public DateTime MinIndoorTempTime { get; set; }
        public int HumidityInsideMax { get; set; }
        public DateTime HumidityInsideMaxTime { get; set; }
        public int HumidityInsideMin { get; set; }
        public DateTime HumidityInsideMinTime { get; set; }
        public decimal MaxOutdoorTemp { get; set; }
        public decimal MinOutdoorTemp { get; set; }
        public DateTime MaxOutdoorTempTime { get; set; }
        public DateTime MinOutdoorTempTime { get; set; }
        public decimal WindChillMin { get; set; }
        public DateTime WindChillMinTime { get; set; }
        public decimal ThswMax { get; set; }
        public DateTime ThswMaxTime { get; set; }
        public decimal HighSolar { get; set; }
        public DateTime HighSolarTime { get; set; }
        public decimal HighUv { get; set; }
        public DateTime HighUvTime { get; set; }


        public override string ToString()
        {
            return this.PrintAllProperties();
        }
    }
}
