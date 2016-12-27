using System;

namespace DavisVantage.WeatherReader
{
    public static class MetricConversion
    {
        public static decimal FahrenheitToDegrees(float fahrenheit)
        {
            return Math.Round(Convert.ToDecimal((fahrenheit - 32) * 5 / 9), 1);
        }
        public static int InHgTohPa(float inHg)
        {
            return Convert.ToInt32(inHg * 33.86389);
        }
        public static int MphToKph(float mph)
        {
            return Convert.ToInt32(mph * 1.609344);
        }
    }
}
