using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DavisVantage.WeatherReader.Models
{
    public class ConsoleInfo
    {
        public decimal IssBatteryStatus { get; set; }
        public decimal ConsoleBatteryVoltage { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in typeof(ConsoleInfo).GetRuntimeProperties())
            {
                stringBuilder.Append($"\n{property.Name}: {property.GetValue(this)}");
            }
            return stringBuilder.ToString();
        }
    }
}
