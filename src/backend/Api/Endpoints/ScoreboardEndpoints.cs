namespace TicTacToe.Api.Endpoints;

using TicTacToe.Application.Services;

public static class ScoreboardEndpoints
{
    public static void MapScoreboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/scoreboard");

        group.MapGet("/", async (IScoreboardService scoreboardService) =>
        {
            var scoreboard = await scoreboardService.GetScoreboardAsync();
            return Results.Ok(scoreboard);
        });

        group.MapPost("/reset", async (IScoreboardService scoreboardService) =>
        {
            var scoreboard = await scoreboardService.ResetScoreboardAsync();
            return Results.Ok(scoreboard);
        });
    }
}
