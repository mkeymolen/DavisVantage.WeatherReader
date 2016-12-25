using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DavisVantage.WeatherReader.Models
{
    public class CurrentWeather
    {
        public int Barometer { get; set; }
        public decimal TempInside { get; set; }
        public int HumidityInside { get; set; }
        public decimal TempOutside { get; set; }
        public int WindSpeed { get; set; }
        public int WindSpeed10Min { get; set; }
        public int WindDirectionDegrees { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in typeof(CurrentWeather).GetRuntimeProperties())
            {
                stringBuilder.Append($"\n{property.Name}: {property.GetValue(this)}");
            }
            return stringBuilder.ToString();
        }
    }
}
