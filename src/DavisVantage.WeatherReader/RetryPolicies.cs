﻿using System;
using Polly;
using Polly.Retry;

namespace DavisVantage.WeatherReader
{
    public class RetryPolicies
    {
        public static RetryPolicy<bool> WakeUpPolicy = Policy
                                                        .Handle<Exception>()
                                                        .OrResult(false)
                                                        .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(3));
    }
}
