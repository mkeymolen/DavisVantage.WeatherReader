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
        public List<decimal> ExtraTemperatures { get; set; }
        public List<decimal> SoilTemperatures { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in typeof(CurrentWeather).GetRuntimeProperties())
            {
                if (property.PropertyType == typeof(List<decimal>))
                {
                    var extraTemperatures = (List<decimal>) property.GetValue(this);
                    for (var i = 0; i < extraTemperatures.Count; i++)
                    {
                        stringBuilder.Append($"\n{property.Name}.{i+1}: {extraTemperatures[i]}");
                    }
                }
                else
                {
                    stringBuilder.Append($"\n{property.Name}: {property.GetValue(this)}");
                }
                
            }
            return stringBuilder.ToString();
        }
    }
}
