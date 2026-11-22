using System;
using System.Collections.Generic;
using System.Text;

namespace PowerPosition.Server
{
    public interface IProcessor
    {
        string CsvFilePath { get; set; }
        Task<List<TotalPerTimePeriod>> GetTotalsAsync();
        void WriteToFile(IEnumerable<TotalPerTimePeriod> totals);

    }
}
