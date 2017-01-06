using System;
using System.Collections.Generic;
using DavisVantage.WeatherReader.Extensions;

namespace DavisVantage.WeatherReader.Models
{
    public class CurrentWeather
    {
        public DateTime UtcTime { get; set; }
        public ConsoleInfo ConsoleInfo { get; set; }
        public int Barometer { get; set; }
        public decimal TempInside { get; set; }
        public decimal TempOutside { get; set; }
        public int HumidityInside { get; set; }
        public int HumidityOutside { get; set; }
        public int WindGust { get; set; }
        public int Wind10MinutesAvg { get; set; }
        public int WindDirection { get; set; }
        public List<decimal> ExtraTemperatures { get; set; }
        public List<decimal> SoilTemperatures { get; set; }
        public List<decimal> LeafTemperatures { get; set; }
        public List<int> ExtraHumidities { get; set; }
        public List<int> SoilMoistures { get; set; } //in cb
        public List<int> LeafWetnesses { get; set; } //scale from 0 - 15
        public decimal RainRate { get; set; }
        public decimal StormRain { get; set; }
        public decimal RainToday { get; set; }
        public int Uv { get; set; }
        public int SolarRadiation { get; set; } // in watt/meter2
        public DateTime SunRise { get; set; }
        public DateTime SunSet { get; set; }

        public override string ToString()
        {
            return this.PrintAllProperties();
        }
    }
}
