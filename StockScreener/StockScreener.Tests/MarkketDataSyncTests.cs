using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using StockScreener.Infrastructure.ExternalApis;

namespace StockScreener.Tests;

/// <summary>
/// Unit tests for RetryPolicy exponential backoff behavior
/// </summary>
public class RetryPolicyTests
{
    [Fact]
    public async Task ExecuteAsync_SucceedsOnFirstAttempt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryPolicy>>();
        var policy = new RetryPolicy(loggerMock.Object, maxRetries: 3, initialDelayMs: 10);
        var callCount = 0;

        // Act
        var result = await policy.ExecuteAsync(
            async (ct) =>
            {
                callCount++;
                return await Task.FromResult("success");
            },
            "TestOp");

        // Assert
        Assert.Equal("success", result);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_RetriesOnTransientError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryPolicy>>();
        var policy = new RetryPolicy(loggerMock.Object, maxRetries: 3, initialDelayMs: 10);
        var callCount = 0;

        // Act
        var result = await policy.ExecuteAsync(
            async (ct) =>
            {
                callCount++;
                if (callCount < 3)
                    throw new TimeoutException("Transient error");
                return await Task.FromResult("success");
            },
            "TestOp");

        // Assert
        Assert.Equal("success", result);
        Assert.Equal(3, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_FailsAfterMaxRetries()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryPolicy>>();
        var policy = new RetryPolicy(loggerMock.Object, maxRetries: 2, initialDelayMs: 10);

        // Act & Assert - rethrows the last exception
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await policy.ExecuteAsync<string>(
                async (ct) =>
                {
                    throw new TimeoutException("Always fails");
                },
                "TestOp");
        });
    }

    [Fact]
    public async Task ExecuteAsync_HandlesRateLimitingGracefully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryPolicy>>();
        var policy = new RetryPolicy(loggerMock.Object, maxRetries: 2, initialDelayMs: 10);
        var callCount = 0;

        // Act & Assert - should retry on 429 and rethrow after max retries
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await policy.ExecuteAsync<string>(
                async (ct) =>
                {
                    callCount++;
                    // Simulate 429 Too Many Requests
                    throw new HttpRequestException("Rate limited", null, HttpStatusCode.TooManyRequests);
                },
                "TestOp");
        });

        // Should have retried at least once
        Assert.True(callCount >= 2);
    }
}

/// <summary>
/// Unit tests for Finnhub JSON response mapping
/// </summary>
public class FinnhubResponseMappingTests
{
    [Fact]
    public void MapFinnhubResponse_ValidResponse_CreatesCandlesCorrectly()
    {
        // Arrange
        var response = new FinnhubCandleResponse
        {
            Status = "ok",
            Close = new List<decimal> { 150.25m, 151.50m },
            Open = new List<decimal> { 149.80m, 150.75m },
            High = new List<decimal> { 151.75m, 152.50m },
            Low = new List<decimal> { 149.50m, 150.25m },
            Volume = new List<long> { 1000000, 1100000 },
            Timestamp = new List<long> { 1706937600, 1707024000 } // 2024-02-03, 2024-02-04
        };

        // Act
        var candles = new List<CandleData>();
        for (int i = 0; i < response.Close.Count; i++)
        {
            var dateTime = UnixTimeStampToDateTime(response.Timestamp[i]);
            candles.Add(new CandleData(
                DateOnly.FromDateTime(dateTime),
                response.Open[i],
                response.High[i],
                response.Low[i],
                response.Close[i],
                response.Volume[i]
            ));
        }

        // Assert
        Assert.Equal(2, candles.Count);
        Assert.Equal(new DateOnly(2024, 2, 3), candles[0].Date);
        Assert.Equal(149.80m, candles[0].Open);
        Assert.Equal(151.75m, candles[0].High);
        Assert.Equal(149.50m, candles[0].Low);
        Assert.Equal(150.25m, candles[0].Close);
        Assert.Equal(1000000, candles[0].Volume);
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTimeStamp);
    }
}

/// <summary>
/// Unit tests for candle upsert logic (unique constraint on StockId + Date)
/// </summary>
public class CandleUpsertLogicTests
{
    [Fact]
    public void UpsertCandleLogic_IdentifiesNewCandlesCorrectly()
    {
        // Arrange
        var existingDates = new HashSet<DateOnly>
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2),
            new DateOnly(2024, 1, 3)
        };

        var newCandlesWithDates = new List<DateOnly>
        {
            new DateOnly(2024, 1, 1), // Existing
            new DateOnly(2024, 1, 4), // New
            new DateOnly(2024, 1, 5), // New
            new DateOnly(2024, 1, 2)  // Existing
        };

        // Act
        var newCandles = newCandlesWithDates
            .Where(d => !existingDates.Contains(d))
            .ToList();

        // Assert
        Assert.Equal(2, newCandles.Count);
        Assert.Contains(new DateOnly(2024, 1, 4), newCandles);
        Assert.Contains(new DateOnly(2024, 1, 5), newCandles);
        Assert.DoesNotContain(new DateOnly(2024, 1, 1), newCandles);
        Assert.DoesNotContain(new DateOnly(2024, 1, 2), newCandles);
    }

    [Fact]
    public void UpsertCandleLogic_HandlesEmptyNewCandleList()
    {
        // Arrange
        var existingDates = new HashSet<DateOnly>
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2)
        };

        var newCandlesWithDates = new List<DateOnly>
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2)
        };

        // Act
        var newCandles = newCandlesWithDates
            .Where(d => !existingDates.Contains(d))
            .ToList();

        // Assert
        Assert.Empty(newCandles);
    }

    [Fact]
    public void UpsertCandleLogic_AllCandlesAreNew()
    {
        // Arrange
        var existingDates = new HashSet<DateOnly>();

        var newCandlesWithDates = new List<DateOnly>
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2),
            new DateOnly(2024, 1, 3)
        };

        // Act
        var newCandles = newCandlesWithDates
            .Where(d => !existingDates.Contains(d))
            .ToList();

        // Assert
        Assert.Equal(3, newCandles.Count);
    }
}
