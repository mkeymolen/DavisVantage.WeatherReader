using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DavisVantage.WeatherReader.Models;
using DavisVantage.WeatherReader.Models.Extremes;

namespace DavisVantage.WeatherReader.WeatherLinkIp
{
    public class WeatherLinkIpByteReader : IByteReader
    {
        public Task<CurrentWeather> ReadCurrentWeatherFromByteArray(byte[] byteArray, bool valuesInMetric)
        {
            var currentWeather = new CurrentWeather();
            currentWeather.UtcTime = DateTime.UtcNow;
            currentWeather.ConsoleInfo = new ConsoleInfo()
            {
                IssBatteryStatus = Convert.ToInt32(byteArray[86]),
                ConsoleBatteryVoltage = Math.Round(Convert.ToDecimal((float)BitConverter.ToInt16(byteArray, 87) * 300 / 512 / 100.0), 1)
            };
            currentWeather.Barometer = GetBarometerValue(byteArray, 7, valuesInMetric);
            currentWeather.TempInside = GetTemperatureValue(byteArray, 9, true);
            currentWeather.HumidityInside = Convert.ToInt32(byteArray[11]);
            currentWeather.TempOutside = GetTemperatureValue(byteArray, 12, true);
            currentWeather.WindSpeed = GetWindSpeedValue(byteArray, 14, valuesInMetric);
            currentWeather.WindSpeed10Min = GetWindSpeedValue(byteArray, 15, valuesInMetric);
            currentWeather.WindDirectionDegrees = BitConverter.ToInt16(byteArray, 16);
            currentWeather.ExtraTemperatures = GetExtraTemperaturesFromBuffer(byteArray, 18, 7, valuesInMetric);
            currentWeather.SoilTemperatures = GetExtraTemperaturesFromBuffer(byteArray, 25, 4, valuesInMetric);
            currentWeather.LeafTemperatures = GetExtraTemperaturesFromBuffer(byteArray, 29, 4, valuesInMetric);
            currentWeather.HumidityOutside = Convert.ToInt32(byteArray[33]);
            currentWeather.ExtraHumidities = GetSingleByteValuesFromBuffer(byteArray, 34, 7);
            var rainRateTicks = (float)BitConverter.ToInt16(byteArray, 41);
            currentWeather.RainRate = valuesInMetric ? Convert.ToDecimal(rainRateTicks / 5) : Convert.ToDecimal(rainRateTicks / 100);
            currentWeather.UvIndex = Convert.ToInt32(byteArray[43]);
            currentWeather.SolarRadiation = BitConverter.ToInt16(byteArray, 44);
            var stormRainTicks = (float)BitConverter.ToInt16(byteArray, 46);
            currentWeather.StormRain = valuesInMetric ? Convert.ToDecimal(stormRainTicks / 5) : Convert.ToDecimal(stormRainTicks / 100);
            var rainToday = (float)BitConverter.ToInt16(byteArray, 50);
            currentWeather.RainToday = valuesInMetric ? Convert.ToDecimal(rainToday / 5) : Convert.ToDecimal(rainToday / 100);
            currentWeather.SoilMoistures = GetSingleByteValuesFromBuffer(byteArray, 62, 4);
            currentWeather.LeafWetnesses = GetSingleByteValuesFromBuffer(byteArray, 66, 4);
            currentWeather.SunRise = GetDateTimeValue(byteArray, 91);
            currentWeather.SunSet = GetDateTimeValue(byteArray, 93);
            return Task.FromResult(currentWeather);
        }
        public Task<WeatherExtremes> ReadWeatherExtremesFromByteArray(byte[] byteArray, bool valuesInMetric)
        {
            var weatherExtremes = new WeatherExtremes();
            var dailyExtremes = new WeatherdayExtremes();
            dailyExtremes.BarometerMin = GetBarometerValue(byteArray, 0, valuesInMetric);
            dailyExtremes.BarometerMax = GetBarometerValue(byteArray, 2, valuesInMetric);
            dailyExtremes.BarometerMinTime = GetDateTimeValue(byteArray, 12);
            dailyExtremes.BarometerMaxTime = GetDateTimeValue(byteArray, 14);
            dailyExtremes.MaxWindSpeed = GetWindSpeedValue(byteArray, 16, valuesInMetric);
            dailyExtremes.MaxWindSpeedTime = GetDateTimeValue(byteArray, 17);
            dailyExtremes.MaxIndoorTemp = GetTemperatureValue(byteArray, 21, valuesInMetric);
            dailyExtremes.MinIndoorTemp = GetTemperatureValue(byteArray, 23, valuesInMetric);
            dailyExtremes.MaxIndoorTempTime = GetDateTimeValue(byteArray, 25);
            dailyExtremes.MinIndoorTempTime = GetDateTimeValue(byteArray, 27);
            dailyExtremes.HumidityInsideMax = Convert.ToInt32(byteArray[37]);
            dailyExtremes.HumidityInsideMin = Convert.ToInt32(byteArray[38]);
            dailyExtremes.HumidityInsideMaxTime = GetDateTimeValue(byteArray, 39);
            dailyExtremes.HumidityInsideMinTime = GetDateTimeValue(byteArray, 41);
            dailyExtremes.MaxOutdoorTemp = GetTemperatureValue(byteArray, 49, valuesInMetric);
            dailyExtremes.MinOutdoorTemp = GetTemperatureValue(byteArray, 47, valuesInMetric);
            dailyExtremes.MaxOutdoorTempTime = GetDateTimeValue(byteArray, 53);
            dailyExtremes.MinOutdoorTempTime = GetDateTimeValue(byteArray, 51);
            dailyExtremes.WindChillMin = GetTemperatureFromSignedBytes(byteArray, 79, valuesInMetric);
            dailyExtremes.WindChillMinTime = GetDateTimeValue(byteArray, 81);
            dailyExtremes.ThswMax = GetTemperatureFromSignedBytes(byteArray, 95, valuesInMetric);
            dailyExtremes.ThswMaxTime= GetDateTimeValue(byteArray, 97);
            dailyExtremes.HighSolar = BitConverter.ToInt16(byteArray, 103);
            dailyExtremes.HighSolarTime = GetDateTimeValue(byteArray, 105);
            dailyExtremes.HighUv = Convert.ToInt32(byteArray[111]);
            dailyExtremes.HighUvTime = GetDateTimeValue(byteArray, 112);

            weatherExtremes.WeatherdayExtremes = dailyExtremes;
            return Task.FromResult(weatherExtremes);
        }

        private int GetBarometerValue(byte[] dataBuffer, int byteOffset, bool valueInMetric)
        {
            var barometerFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, byteOffset) / 1000;
            return valueInMetric ? MetricConversion.InHgTohPa(barometerFromDataBuffer) : Convert.ToInt32(barometerFromDataBuffer);
        }

        private decimal GetTemperatureValue(byte[] dataBuffer, int byteOffset, bool valueInMetric)
        {
            var tempFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, byteOffset) / 10;
            return valueInMetric ? MetricConversion.FahrenheitToDegrees(tempFromDataBuffer) : Convert.ToDecimal(tempFromDataBuffer);
        }

        private decimal GetTemperatureFromSignedBytes(byte[] dataBuffer, int byteOffset, bool valueInMetric)
        {
            var tempFromDataBuffer = (float)BitConverter.ToInt16(dataBuffer, byteOffset);
            return valueInMetric ? MetricConversion.FahrenheitToDegrees(tempFromDataBuffer) : Convert.ToDecimal(tempFromDataBuffer);
        }
        private int GetWindSpeedValue(byte[] dataBuffer, int byteOffset, bool valueInMetric)
        {
            var windFromDataBuffer = Convert.ToInt32(dataBuffer[byteOffset]);
            return valueInMetric ? MetricConversion.MphToKph(windFromDataBuffer) : windFromDataBuffer;
        }

        private DateTime GetDateTimeValue(byte[] dataBuffer, int byteOffset)
        {
            var timeValueInBuffer = (float) BitConverter.ToInt16(dataBuffer, byteOffset);
            return DateTime.Today.AddHours(Math.Floor(timeValueInBuffer / 100)).AddMinutes(timeValueInBuffer % 100);
        }

        private List<decimal> GetExtraTemperaturesFromBuffer(byte[] dataBuffer, int byteOffset, int byteLength, bool valuesInMetric)
        {
            var extraTemperatures = new List<decimal>();
            for (var i = byteOffset; i < byteOffset + byteLength; i++)
            {
                var byteValue = Convert.ToInt32(dataBuffer[i]);
                // ignore max byte values --> no sensor
                if (byteValue != byte.MaxValue)
                {
                    var temperature = byteValue - 90;
                    extraTemperatures.Add(valuesInMetric ? MetricConversion.FahrenheitToDegrees(temperature) : temperature);
                }
            }
            return extraTemperatures;
        }
        private List<int> GetSingleByteValuesFromBuffer(byte[] dataBuffer, int byteOffset, int byteLength)
        {
            var listValues = new List<int>();
            for (var i = byteOffset; i < byteOffset + byteLength; i++)
            {
                var byteValue = Convert.ToInt32(dataBuffer[i]);
                // ignore max byte values --> no sensor
                if (byteValue != byte.MaxValue)
                {
                    listValues.Add(byteValue);
                }
            }
            return listValues;
        }
    }
}
