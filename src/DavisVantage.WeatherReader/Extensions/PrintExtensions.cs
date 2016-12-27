using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DavisVantage.WeatherReader.Extensions
{
    public static class PrintExtensions
    {
        public static string PrintAllProperties<T>(this T data) where T: class 
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in typeof(T).GetRuntimeProperties())
            {
                if (property.PropertyType == typeof(List<decimal>))
                {
                    var extraValues = (List<decimal>)property.GetValue(data);
                    PrintValues(property, ref stringBuilder, extraValues);
                }
                else if (property.PropertyType == typeof(List<int>))
                {
                    var extraValues = (List<int>)property.GetValue(data);
                    PrintValues(property, ref stringBuilder, extraValues);
                }
                else
                {
                    stringBuilder.Append($"\n{property.Name}: {property.GetValue(data)}");
                }
            }
            return stringBuilder.ToString();
        }

        private static void PrintValues<T>(PropertyInfo propertyInfo, ref StringBuilder stringBuilder, List<T> extraValues)
        {
            for (var i = 0; i < extraValues.Count; i++)
            {
                stringBuilder.Append($"\n{propertyInfo.Name}[{i}]: {extraValues[i]}");
            }
        }
    }
}
