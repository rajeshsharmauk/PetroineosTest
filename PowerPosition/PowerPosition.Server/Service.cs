using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Polly;

namespace PowerPosition.Server
{
    /// <summary>
    /// Provides methods to call the PowerService.
    /// </summary>
    public class Service : IService
    {
        private readonly PowerService _powerService;
        private readonly ILogger<Service> _logger;
        TimeSpan[] _retryIntervals;

        public Service(ILogger<Service> logger)
        {
            _powerService = new PowerService();
            _logger = logger;
            _retryIntervals = new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) };
        }
        /// <summary>
        /// Fetches trades from the PowerService.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            try
            {
                return _powerService.GetTrades(date);
            }
            catch (PowerServiceException e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
        /// <summary>
        /// Fetches trades from the PowerService asynchronously.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// 
        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            try
            {
                // Create a retry policy using Polly.
                var retryPolicy = Policy.Handle<PowerServiceException>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning($"Retry {retryCount} due to: {exception.Message}. Waiting {timeSpan} before next retry.");
                        });

                return await retryPolicy.ExecuteAsync(async () =>
                {
                    var trades = await _powerService.GetTradesAsync(date);
                    return trades;
                });
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.Flatten().InnerExceptions)
                {
                    if (innerException is PowerServiceException powerException)
                    {
                        _logger.LogError(powerException.Message);
                    }
                    else
                    {
                        _logger.LogError(innerException.Message);
                    }
                }

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError("Retry failure in getting trades from PowerService: " + e.Message);
                throw;
            }
        }
    }
}