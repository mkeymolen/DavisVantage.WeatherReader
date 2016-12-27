using DavisVantage.WeatherReader.Extensions;

namespace DavisVantage.WeatherReader.Models.Extremes
{
    public class WeatherExtremes
    {
        public WeatherDayExtremes WeatherDayExtremes { get; set; }
        public WeatherMonthExtremes WeatherMonthExtremes { get; set; }
        public WeatherYearExtremes WeatherYearExtremes { get; set; }
        public override string ToString()
        {
            return this.PrintAllProperties();
        }
    }
}
