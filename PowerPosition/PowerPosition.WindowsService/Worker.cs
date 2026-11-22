using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PowerPosition.Server;

namespace PowerPosition.WindowsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ApplicationSettings _settings;
        private IProcessor _processor;
        private System.Timers.Timer _timer;

        public Worker(IProcessor processor, ILogger<Worker> logger, IOptions<ApplicationSettings> options)
        {
            _processor = processor;
            _logger = logger;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(1000, stoppingToken);

                // Retrieve file path from appsettings.
                string directory = _settings.CsvFilePath;

                // Create directory if it doesn't already exist.
                Directory.CreateDirectory(directory);

                SetTimer();
            }
        }
        /// <summary>
        /// Creates and starts the timer.
        /// </summary>
        private void SetTimer()
        {
            _timer = new System.Timers.Timer()
            {
                Enabled = true
            };

            _timer.Elapsed += async (sender, args) =>
            {
                try
                {
                    // Get timer interval from app.config.
                    _timer.Interval = _settings.TimerInterval;

                    // Retrieve totals asynchronously.
                    List<TotalPerTimePeriod> totals = await _processor.GetTotalsAsync();

                    // Write totals to file.
                    if (totals.Any())
                    {
                        _processor.WriteToFile(totals);
                    }
                }
                catch (Exception e)
                {
                    // Log an error and carry on.
                    _logger.LogError(e, "An error occurred while processing power trades.");
                }
            };
        }
    }
}