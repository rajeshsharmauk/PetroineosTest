using Microsoft.Extensions.Logging;
using Moq;
using PowerPosition.Server;
using Services;
using FluentAssertions;

namespace PowerPosition.Tests;

public class ServiceTests
{  
    private readonly ILogger<Service> _logger;

    public ServiceTests()
    {
        _logger = new Mock<ILogger<Service>>().Object;
    }

    [Fact]
    public void GetTrades_PowerTradeContainsCorrectNumberOfPeriods()
    {
        var expected = 24;
        var service = new Service(_logger);

        var result = service.GetTrades(DateTime.Today).ToList();

        result[0].Periods.Count().Should().Be(expected);
    }

    [Fact]
    public async Task GetTradesAsync_PowerTradeContainsCorrectNumberOfPeriods()
    {
        var expected = 24;
        var service = new Service(_logger);

        var result = (await service.GetTradesAsync(DateTime.Today)).ToList();

        result[0].Periods.Count().Should().Be(expected);
    }

    [Fact]
    public async Task GetTrades_PowerTradeContainsCorrectDate()
    {
        var expected = DateTime.Today;
        var service = new Service(_logger);

        var result = service.GetTrades(DateTime.Today).ToList();

        result[0].Date.Should().Be(expected);
    }

    [Fact]
    public async Task GetTradesAsync_PowerTradeContainsCorrectDate()
    {
        var expected = DateTime.Today;
        var service = new Service(_logger);

        var result = (await service.GetTradesAsync(DateTime.Today)).ToList();

        result[0].Date.Should().Be(expected);
    }
}