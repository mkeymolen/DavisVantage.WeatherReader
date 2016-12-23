using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace DavisVantage.WeatherReader
{
    public class RetryPolicies
    {
        public static RetryPolicy<bool> WakeUpPolicy = Policy
                                                        .Handle<Exception>()
                                                        .OrResult(false)
                                                        .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(1));
    }
}
