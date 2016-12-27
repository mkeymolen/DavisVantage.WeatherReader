using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader
{
    public interface IByteReader
    {
        CurrentWeather ReadCurrentWeatherFromByteArray(byte[] byteArray, bool valuesInMetric);
        WeatherExtremes ReadWeatherExtremesFromByteArray(byte[] byteArray, bool valuesInMetric);
    }
}
