using Microsoft.Extensions.Logging;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Implements exponential backoff retry strategy with rate limit handling
/// </summary>
public class RetryPolicy
{
    private readonly ILogger<RetryPolicy> _logger;
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;

    public RetryPolicy(
        ILogger<RetryPolicy> logger,
        int maxRetries = 4,
        int initialDelayMs = 1000)
    {
        _logger = logger;
        _maxRetries = maxRetries;
        _initialDelay = TimeSpan.FromMilliseconds(initialDelayMs);
    }

    /// <summary>
    /// Execute an async operation with exponential backoff retry
    /// </summary>
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        string operationName,
        CancellationToken ct = default)
    {
        TimeSpan delay = _initialDelay;

        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return await operation(ct);
            }
            catch (HttpRequestException ex) when (
                ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                (int?)ex.StatusCode == 429)
            {
                if (attempt == _maxRetries)
                {
                    _logger.LogError(
                        "Rate limit exceeded after {Attempts} retries for {Operation}",
                        attempt + 1,
                        operationName);
                    throw;
                }

                _logger.LogWarning(
                    "Rate limit hit (429) for {Operation}. Retrying in {DelayMs}ms (attempt {Attempt}/{MaxRetries})",
                    operationName,
                    (int)delay.TotalMilliseconds,
                    attempt + 1,
                    _maxRetries);

                await Task.Delay(delay, ct);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
            catch (Exception ex) when (attempt < _maxRetries && IsTransientError(ex))
            {
                _logger.LogWarning(
                    "Transient error in {Operation}: {Message}. Retrying in {DelayMs}ms (attempt {Attempt}/{MaxRetries})",
                    operationName,
                    ex.Message,
                    (int)delay.TotalMilliseconds,
                    attempt + 1,
                    _maxRetries);

                await Task.Delay(delay, ct);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
        }

        throw new InvalidOperationException($"Operation {operationName} failed after {_maxRetries} retries");
    }

    private static bool IsTransientError(Exception ex) =>
        ex is HttpRequestException or TimeoutException or OperationCanceledException;
}
