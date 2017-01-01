using System.IO;
using Microsoft.Extensions.Configuration;

namespace Sample.NetCore.Autofac
{
    public class ConfigHelper
    {
        public static T Read<T>(string configKey)
        {
            var configurationBuilder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json");
            var configFile = configurationBuilder.Build();
            return configFile.GetSection(configKey).Get<T>();
        }
    }
}
