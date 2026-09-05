namespace TicTacToe.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Repositories;
using TicTacToe.Domain.ValueObjects;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games");

        group.MapPost("/", async (CreateGameRequest request, IGameRepository gameRepository) =>
        {
            var mode = Enum.TryParse<GameMode>(request.Mode, true, out var parsedMode)
                ? parsedMode
                : GameMode.TwoPlayer;

            var game = Game.Create(mode);
            await gameRepository.AddAsync(game);

            return Results.Created($"/api/games/{game.Id}", game.ToDto());
        });

        group.MapGet("/{id:guid}", async (Guid id, IGameRepository gameRepository) =>
        {
            var game = await gameRepository.GetByIdAsync(id);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }

            return Results.Ok(game.ToDto());
        });

        group.MapPost("/{id:guid}/moves", async (
            Guid id,
            [FromBody] MakeMoveRequest request,
            IGameRepository gameRepository,
            IScoreboardRepository scoreboardRepository) =>
        {
            if (request.Row < 1 || request.Row > 3 || request.Column < 1 || request.Column > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(request), "Row and Column must be between 1 and 3.");
            }

            if (!Enum.TryParse<Player>(request.Player, true, out var player))
            {
                throw new ArgumentException($"Invalid player mark '{request.Player}'. Must be 'X' or 'O'.");
            }

            var game = await gameRepository.GetByIdAsync(id);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }

            var position = CellPosition.FromOneBased(request.Row, request.Column);
            game.MakeMove(player, position);

            if (game.Status == GameStatus.Won && game.Winner.HasValue)
            {
                var scoreboard = await scoreboardRepository.GetScoreboardAsync();
                scoreboard.RecordWin(game.Winner.Value);
                await scoreboardRepository.UpdateAsync(scoreboard);
            }
            else if (game.Status == GameStatus.Draw)
            {
                var scoreboard = await scoreboardRepository.GetScoreboardAsync();
                scoreboard.RecordDraw();
                await scoreboardRepository.UpdateAsync(scoreboard);
            }

            await gameRepository.UpdateAsync(game);

            return Results.Ok(game.ToDto());
        });
    }
}
