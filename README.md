
# DavisVantage.WeatherReader [![Build status](https://ci.appveyor.com/api/projects/status/2w2e1oa30glhatvc/branch/master?svg=true)](https://ci.appveyor.com/project/mkeymolen/davisvantage-weatherreader/branch/master) [![NuGet](https://img.shields.io/nuget/vpre/DavisVantage.WeatherReader.svg)](https://www.nuget.org/packages/DavisVantage.WeatherReader/) [![license](https://img.shields.io/github/license/mkeymolen/DavisVantage.WeatherReader.svg)](https://github.com/mkeymolen/DavisVantage.WeatherReader/blob/master/LICENSE) ![](https://img.shields.io/badge/.net-4.5.1-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.6-yellowgreen.svg)
Davis Vantage WeatherReader is a **crossplatform** [.netstandard 1.6](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) and .NETFramework 4.5.1 library that reads weather data from a Davis Vantage weatherstation. It follows the official Vantage Pro, Pro2, and Vue Communications Reference which can be downloaded at [http://www.davisnet.com/support/vantage-pro-pro2-and-vue-communications-reference/](http://www.davisnet.com/support/vantage-pro-pro2-and-vue-communications-reference/)

## Features
- Connecting and reading live weatherdata from a Davis Vantage pro(2 plus) console
- Dynamic logging mechanism by using [liblog](https://github.com/damianh/LibLog)

## What do you need?
1. Davis Vantage Weatherstation with console
2. Davis Vantage Datalogger

**Current supported dataloggers:**

-  [**Weatherlink** **IP** **Data** **logger**](http://www.davisnet.com/product/weatherlinkip-for-vantage-stations/)

## How to use
- Import the latest DavisVantage.WeatherReader package from **[nuget](https://www.nuget.org/packages/DavisVantage.WeatherReader/).** 
- Register the the implementations with your preferred IOC container.

Example with autofac:

	builder.Register(c => weatherlinkIpSettings).As<IDataLoggerSettings>();	
	builder.RegisterType<WeatherLinkIpByteReader>().As<IByteReader>();	
	builder.RegisterType<WeatherLinkIpDataLogger>().As<IDataLogger<WeatherLinkIpSettings>>();	

Connect to the console:

    using (var dataLogger = scope.Resolve<IDataLogger<WeatherLinkIpSettings>>())
    {
    	if (dataLogger.Connect())
    	{
    		var currentWeather = dataLogger.ReadCurrentWeather(true);
    		var weatherExtremes = dataLogger.ReadWeatherExtremes(true);
    	}
    }


## Samples
The included samples are working with a [**Weatherlink** **IP** **Data** **logger**](http://www.davisnet.com/product/weatherlinkip-for-vantage-stations/) and a [**Davis** **Vantage** **Pro2** **Plus**](http://www.davisnet.com/solution/vantage-pro2-plus/) weatherstation. 
### .netcore sample
- Using NLog for logging.

### .netcore autofac sample
- Using NLog for logging and autofac as IOC container.
- Settings are read from appsettings.json