using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader
{
    public interface IDataLogger<in T> where T : IDataLoggerSettings
    {
        /// <summary>
        /// Connect to Console
        /// </summary>
        /// <param name="settings">dataloggersettings of type IDataLoggerSettings</param>
        /// <returns>Connection succeeded or not</returns>
        bool Connect(T settings);
        /// <summary>
        /// WakeUp console. Not necessary to call this method seperately
        /// </summary>
        bool WakeUp();
        /// <summary>
        /// Read current weather data from console
        /// </summary>
        /// <param name="valuesInMetric">Specify if returned values should be converted to metric or to keep imperial</param>
        /// <returns>CurrentWeather object with ConsoleInfo included</returns>
        CurrentWeather ReadCurrentWeather(bool valuesInMetric);

        /// <summary>
        /// Read extremes from console
        /// </summary>
        /// <param name="valuesInMetric">Specify if returned values should be converted to metric or to keep imperial</param>
        /// <returns>WeatherExtremes object that includes day, month and year data</returns>
        WeatherExtremes ReadWeatherExtremes(bool valuesInMetric);
    }
}
