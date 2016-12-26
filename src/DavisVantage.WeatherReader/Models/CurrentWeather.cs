using System;
using System.Collections;
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
        public decimal TempOutside { get; set; }
        public int HumidityInside { get; set; }
        public int HumidityOutside { get; set; }
        public int WindSpeed { get; set; }
        public int WindSpeed10Min { get; set; }
        public int WindDirectionDegrees { get; set; }
        public List<decimal> ExtraTemperatures { get; set; }
        public List<decimal> SoilTemperatures { get; set; }
        public List<decimal> LeafTemperatures { get; set; }
        public List<int> ExtraHumidities { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in typeof(CurrentWeather).GetRuntimeProperties())
            {
                if (property.PropertyType == typeof(List<decimal>))
                {
                    var extraValues = (List<decimal>)property.GetValue(this);
                    PrintValues(property, ref stringBuilder, extraValues);
                }
                else if (property.PropertyType == typeof(List<int>))
                {
                    var extraValues = (List<int>)property.GetValue(this);
                    PrintValues(property, ref stringBuilder, extraValues);
                }
                else
                {
                    stringBuilder.Append($"\n{property.Name}: {property.GetValue(this)}");
                }

            }
            return stringBuilder.ToString();
        }

        private void PrintValues<T>(PropertyInfo propertyInfo, ref StringBuilder stringBuilder, List<T> extraValues)
        {
            for (var i = 0; i < extraValues.Count; i++)
            {
                stringBuilder.Append($"\n{propertyInfo.Name}[{i}]: {extraValues[i]}");
            }
        }
    }
}
