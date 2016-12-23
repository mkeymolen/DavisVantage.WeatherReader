using System.Threading.Tasks;

namespace DavisVantage.WeatherReader
{
    public interface IDataLogger<in T> where T : IDataLoggerSettings
    {
        void Connect(T settings);
        void WakeUp();
        Task ReadCurrentWeather();
        Task ReadDayExtremes();
    }
}
