using DavisVantage.WeatherReader.Extensions;

namespace DavisVantage.WeatherReader.Models
{
    public class ConsoleInfo
    {
        public decimal IssBatteryStatus { get; set; }
        public decimal ConsoleBatteryVoltage { get; set; }

        public override string ToString()
        {
            return this.PrintAllProperties();
        }
    }
}
