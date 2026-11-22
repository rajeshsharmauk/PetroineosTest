using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using PowerPosition.Server;
using Services;

namespace PowerPosition.Tests
{
    public class ProcessorTests
    {
        private readonly ILogger<Processor> _logger = new Mock<ILogger<Processor>>().Object;
        private readonly IConfiguration _configuration;
        private Mock<IConfiguration> _configMock = new Mock<IConfiguration>();

        public ProcessorTests()
        {
            _logger = new Mock<ILogger<Processor>>().Object;

            _configMock.Setup(x => x["ApplicationSettings:CsvFilePath"]).Returns("Output");
            _configuration = _configMock.Object;
        }

        [Fact]
        public async Task GetTotals_WithNoTrades_ReturnsEmptyList()
        {
            var processor = new Processor(new MockedService(Array.Empty<PowerTrade>()), _logger, _configuration);

            var result = await processor.GetTotalsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTotals_WhenServiceFails_ExceptionShoudBeThrown()
        {
            var message = "Error retrieving power volumes";
            var expected = new PowerServiceException(message);
            var processor = new Processor(new MockedService(expected), _logger, _configuration);

            var exception = await Assert.ThrowsAsync<PowerServiceException>(() => processor.GetTotalsAsync());

            if (exception.InnerException != null)
            {
                exception.InnerException.Should().Be(expected.Message);
            }
            else
            {
                exception.InnerException.Should().Be(null);
            }
        }

        [Fact]
        public void WriteToFile_WritesCsvHeaderAndTotals()
        {
            var directory = Path.Combine(Path.GetTempPath(), "PowerPositionTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);

            try
            {
                var processor = new Processor(new MockedService(Array.Empty<PowerTrade>()), _logger, _configuration)
                {
                    CsvFilePath = directory + Path.DirectorySeparatorChar
                };

                processor.WriteToFile(new[]
                {
                    new TotalPerTimePeriod { TimePeriod = "23:00", Total = 12.5 }
                });

            var file = Directory.GetFiles(directory, "PowerPosition_*.csv").Single();
            var lines = File.ReadAllLines(file);

            Assert.Equal("Local Time,Volume", lines[0]);
            Assert.Equal("23:00,12.5", lines[1]);

            }
            finally
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, recursive: true);
                }
            }
        }
    }
}