using PowerPosition.Server;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerPosition.Tests
{
    public sealed class MockedService : IService
    {
        private readonly IEnumerable<PowerTrade>? _trades;
        private readonly Exception? _exception;

        public MockedService(IEnumerable<PowerTrade> trades)
        {
            _trades = trades;
        }

        public MockedService(Exception exception)
        {
            _exception = exception;
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date) => throwIfNeeded() ?? _trades!;

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            return Task.FromResult(GetTrades(date));
        }

        private IEnumerable<PowerTrade>? throwIfNeeded()
        {
            if (_exception is not null)
            {
                throw _exception;
            }

            return null;
        }
    }
}
