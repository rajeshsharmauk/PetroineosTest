using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace PowerPosition.Server
{
    /// <summary>
    /// Provides methods to call the PowerService.
    /// </summary>
    public class Service : IService
    {
        private readonly PowerService _powerService;
        private readonly ILogger<Service> _logger;

        public Service(ILogger<Service> logger)
        {
            _powerService = new PowerService();
            _logger = logger;
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
                return await Task.Run(() => _powerService.GetTrades(date));
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
        }
    }
}