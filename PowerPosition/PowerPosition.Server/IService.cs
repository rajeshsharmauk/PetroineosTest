using System;
using System.Collections.Generic;
using System.Text;
using Services;

namespace PowerPosition.Server
{
    public interface IService
    {
        IEnumerable<PowerTrade> GetTrades(DateTime date);
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);

    }
}
