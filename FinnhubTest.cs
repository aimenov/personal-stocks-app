using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace FinnhubTest;

public class FinnhubCandleResponse
{
    public string s { get; set; } = string.Empty;
    public System.Collections.Generic.List<decimal> c { get; set; } = new();
    public System.Collections.Generic.List<decimal> o { get; set; } = new();
    public System.Collections.Generic.List<decimal> h { get; set; } = new();
    public System.Collections.Generic.List<decimal> l { get; set; } = new();
    public System.Collections.Generic.List<long> v { get; set; } = new();
    public System.Collections.Generic.List<long> t { get; set; } = new();
}

class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = "d5pjgtpr01qlfcaed6c0d5pjgtpr01qlfcaed6cg";
        var ticker = "MSFT";
        
        // Calculate dates
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var fromDate = toDate.AddDays(-30); // Test with 30 days instead of 365
        
        var fromUnix = ((DateTimeOffset)fromDate.ToDateTime(TimeOnly.MinValue)).ToUnixTimeSeconds();
        var toUnix = ((DateTimeOffset)toDate.ToDateTime(TimeOnly.MinValue)).ToUnixTimeSeconds();
        
        var url = $"https://finnhub.io/api/v1/stock/candle?symbol={ticker}&resolution=D&from={fromUnix}&to={toUnix}&token={apiKey}";
        
        Console.WriteLine($"Testing Finnhub API...");
        Console.WriteLine($"URL: {url}");
        Console.WriteLine($"From: {fromDate:yyyy-MM-dd} ({fromUnix})");
        Console.WriteLine($"To:   {toDate:yyyy-MM-dd} ({toUnix})");
        Console.WriteLine();
        
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(10);
        
        try
        {
            Console.WriteLine("Sending request...");
            var response = await client.GetAsync(url);
            
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Headers: {response.Headers}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response length: {json.Length} chars");
                Console.WriteLine($"First 500 chars: {json.Substring(0, Math.Min(500, json.Length))}");
                
                var data = JsonSerializer.Deserialize<FinnhubCandleResponse>(json);
                Console.WriteLine();
                Console.WriteLine($"Status: {data?.s}");
                Console.WriteLine($"Candles: {data?.c?.Count ?? 0}");
                Console.WriteLine($"Timestamps: {data?.t?.Count ?? 0}");
                
                if (data?.c?.Count > 0)
                {
                    Console.WriteLine($"First candle: Date={UnixToDate(data.t[0]):yyyy-MM-dd}, Open={data.o[0]}, High={data.h[0]}, Low={data.l[0]}, Close={data.c[0]}, Vol={data.v[0]}");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
    }
    
    static DateTime UnixToDate(long unixTime)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTime);
    }
}
