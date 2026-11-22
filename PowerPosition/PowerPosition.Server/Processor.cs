using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace PowerPosition.Server
{
    /// <summary>
    /// Provides the main processing for retrieving data from
    /// the PowerService and writing to a csv file.
    /// </summary>
    public class Processor : IProcessor
    {
        private IService _service;

        private readonly ILogger<Processor> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Storage for time periods.
        /// </summary>
        private readonly Dictionary<int, string> _timePeriods;

        /// <summary>
        /// File to be written to.
        /// </summary>
        public OutputFile _outputFile;

        /// <summary>
        /// Path of the output file.
        /// </summary>
        public string CsvFilePath { get; set; }

        public Processor(IService service, ILogger<Processor> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _timePeriods = CreateTimePeriods();
            CsvFilePath = _configuration["ApplicationSettings:CsvFilePath"] ?? "Output";
        }

        /// <summary>
        /// Fetch totals from the PowerService.
        /// </summary>
        /// <returns>A list of TotalPerTimePeriod objects.</returns>
        public async Task<List<TotalPerTimePeriod>> GetTotalsAsync()
        {
            var totalsPerTimePeriod = new List<TotalPerTimePeriod>();

            try
            {
                // Asychronously fetch trades from the power service.
                IEnumerable<PowerTrade> powerTrades = await _service.GetTradesAsync(DateTime.Today);

                // Flatten the result to get the data in periods.
                IEnumerable<PowerPeriod> periods = powerTrades.SelectMany(x => x.Periods);

                // Obtain the sum of volumes per period.
                var result = periods.GroupBy(x => x.Period).Select(y => new
                {
                    Period = y.Key,
                    Total = y.Sum(z => z.Volume)
                });

                foreach (var item in result)
                {
                    string timePeriod = string.Empty;

                    // Fetch a time period from a numerical value.
                    if (!_timePeriods.TryGetValue(item.Period, out timePeriod))
                    {
                        _logger.LogError($"Time period {item.Period} not found in dictionary");
                    }

                    // Create an object to store the time period and it's total volume.
                    TotalPerTimePeriod totalPerTimePeriod = new TotalPerTimePeriod
                    {
                        TimePeriod = timePeriod,
                        Total = Math.Round(item.Total, 2)
                    };

                    // Add to list.
                    totalsPerTimePeriod.Add(totalPerTimePeriod);
                }
            }
            catch (PowerServiceException e)
            {
                _logger.LogError(e.ToString());
                throw;
            }

            return totalsPerTimePeriod;
        }
        /// <summary>
        /// Initiates the writing of totals to a file.
        /// </summary>
        /// <param name="totals">A list of totals per time period.</param>
        public void WriteToFile(IEnumerable<TotalPerTimePeriod> totals)
        {
            string filename = Path.Combine(CsvFilePath, $"PowerPosition_{DateTime.Today.ToString("yyyyMMdd")}_{DateTime.Now:HHmm}.csv");

            _outputFile = new OutputFile(filename);
            _outputFile.Write(totals);
        }
        /// <summary>
        /// Creates a dictionary with a mapping between a numerical time period
        /// and it's string representation.
        /// </summary>
        /// <returns>Dictionary of integer-to-string mappings.</returns>
        private Dictionary<int, string> CreateTimePeriods()
        {
            Dictionary<int, string> timePeriods = new Dictionary<int, string>
            {
                {1, "23:00"},
                {2, "24:00"},
                {3, "01:00"},
                {4, "02:00"},
                {5, "03:00"},
                {6, "04:00"},
                {7, "05:00"},
                {8, "06:00"},
                {9, "07:00"},
                {10, "08:00"},
                {11, "09:00"},
                {12, "10:00"},
                {13, "11:00"},
                {14, "12:00"},
                {15, "13:00"},
                {16, "14:00"},
                {17, "15:00"},
                {18, "16:00"},
                {19, "17:00"},
                {20, "18:00"},
                {21, "19:00"},
                {22, "20:00"},
                {23, "21:00"},
                {24, "22:00"}
            };

            return timePeriods;
        }
    }
}