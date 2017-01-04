using System;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader
{
    public interface IDataLogger<T> : IDisposable where T : IDataLoggerSettings
    {
        T Settings { get; set; }
        /// <summary>
        /// Connect to Console
        /// </summary>
        /// <returns>Connection succeeded or not</returns>
        bool Connect();
        /// <summary>
        /// WakeUp console. Not necessary to call this method seperately
        /// </summary>
        bool WakeUp();
        /// <summary>
        /// Read current weather data from console
        /// </summary>
        /// <param name="valuesInMetric">Specify if returned values should be converted to metric or to keep imperial</param>
        /// <returns>CurrentWeather object with ConsoleInfo included</returns>
        Task<CurrentWeather> ReadCurrentWeather(bool valuesInMetric);

        /// <summary>
        /// Read extremes from console
        /// </summary>
        /// <param name="valuesInMetric">Specify if returned values should be converted to metric or to keep imperial</param>
        /// <returns>WeatherExtremes object that includes day, month and year data</returns>
        Task<WeatherExtremes> ReadWeatherExtremes(bool valuesInMetric);
    }
}
