using StockScreener.Application.DTOs;
using StockScreener.Application.Interfaces;

namespace StockScreener.API.Endpoints;

/// <summary>
/// API endpoints for stock screening operations
/// </summary>
public static class ScreenerEndpoints
{
    public static void MapScreenerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/screener")
            .WithName("Screener");

        group.MapPost("/run", RunScreener)
            .WithName("Run Screener")
            .WithDescription("Execute a complete screening run across the stock universe")
            .Produces<List<ScreenResultDto>>(StatusCodes.Status200OK);

        group.MapGet("/top/{count}", GetTopResults)
            .WithName("Get Top Results")
            .WithDescription("Get the top N stocks from the latest screening run")
            .Produces<List<ScreenResultDto>>(StatusCodes.Status200OK);

        group.MapGet("/missed-opportunities", GetMissedOpportunities)
            .WithName("Get Missed Opportunities")
            .WithDescription("Find stocks that were in top N previously but not in the latest run")
            .Produces<List<ScreenResultDto>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> RunScreener(
        IScreeningService screeningService,
        CancellationToken ct)
    {
        try
        {
            var results = await screeningService.RunScreenerAsync(ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetTopResults(
        int count,
        IScreeningService screeningService,
        CancellationToken ct)
    {
        try
        {
            if (count <= 0)
            {
                return Results.BadRequest(new { error = "Count must be greater than 0" });
            }

            var results = await screeningService.GetTopResultsAsync(count, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetMissedOpportunities(
        int daysLookback = 30,
        int topN = 50,
        IScreeningService screeningService = null!,
        CancellationToken ct = default)
    {
        try
        {
            if (daysLookback <= 0)
            {
                return Results.BadRequest(new { error = "daysLookback must be greater than 0" });
            }

            if (topN <= 0)
            {
                return Results.BadRequest(new { error = "topN must be greater than 0" });
            }

            var results = await screeningService.GetMissedOpportunitiesAsync(daysLookback, topN, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
