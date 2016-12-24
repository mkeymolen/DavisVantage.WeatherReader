using System.Threading.Tasks;
using DavisVantage.WeatherReader.Models;

namespace DavisVantage.WeatherReader
{
    public interface IDataLogger<in T> where T : IDataLoggerSettings
    {
        bool Connect(T settings);
        bool WakeUp();
        CurrentWeather ReadCurrentWeather();
        void ReadDayExtremes();
    }
}
