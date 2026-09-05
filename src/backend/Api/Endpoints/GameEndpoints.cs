namespace TicTacToe.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Services;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games");

        group.MapPost("/", async (CreateGameRequest request, IGameService gameService) =>
        {
            var game = await gameService.CreateGameAsync(request);
            return Results.Created($"/api/games/{game.Id}", game);
        });

        group.MapGet("/{id:guid}", async (Guid id, IGameService gameService) =>
        {
            var game = await gameService.GetGameByIdAsync(id);
            return Results.Ok(game);
        });

        group.MapPost("/{id:guid}/moves", async (
            Guid id,
            [FromBody] MakeMoveRequest request,
            IGameService gameService) =>
        {
            var game = await gameService.MakeMoveAsync(id, request);
            return Results.Ok(game);
        });

        group.MapPost("/{id:guid}/reset", async (Guid id, IGameService gameService) =>
        {
            var game = await gameService.ResetGameAsync(id);
            return Results.Ok(game);
        });

        group.MapPost("/{id:guid}/undo", async (Guid id, IGameService gameService) =>
        {
            var game = await gameService.UndoMoveAsync(id);
            return Results.Ok(game);
        });
    }
}
