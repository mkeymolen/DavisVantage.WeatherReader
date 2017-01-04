using System.Threading.Tasks;
using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader
{
    public interface IByteReader
    {
        Task<CurrentWeather> ReadCurrentWeatherFromByteArray(byte[] byteArray, bool valuesInMetric);
        Task<WeatherExtremes> ReadWeatherExtremesFromByteArray(byte[] byteArray, bool valuesInMetric);
    }
}
