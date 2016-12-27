using System;
using DavisVantage.WeatherReader.Extensions;

namespace DavisVantage.WeatherReader.Models.Extremes
{
    public class WeatherDayExtremes
    {
        public int BarometerMax { get; set; }
        public int BarometerMin { get; set; }
        public DateTime BarometerMaxTime { get; set; }
        public DateTime BarometerMinTime { get; set; }
        public int MaxWindSpeed { get; set; }
        public DateTime MaxWindSpeedTime { get; set; }
        public decimal TempInsideMax { get; set; }
        public DateTime TempInsideMaxTime { get; set; }
        public decimal TempInsideMin { get; set; }
        public DateTime TempInsideMinTime { get; set; }
        public int HumidityInsideMax { get; set; }
        public DateTime HumidityInsideMaxTime { get; set; }
        public int HumidityInsideMin { get; set; }
        public DateTime HumidityInsideMinTime { get; set; }
        public decimal TempOutsideMax { get; set; }
        public decimal TempOutsideMin { get; set; }
        public DateTime TempOutsideMaxTime { get; set; }
        public DateTime TempOutsideMinTime { get; set; }
        public decimal WindChillMin { get; set; }
        public DateTime WindChillMinTime { get; set; }
        public decimal ThswMax { get; set; }
        public DateTime ThswMaxTime { get; set; }

        public override string ToString()
        {
            return this.PrintAllProperties();
        }
    }
}
