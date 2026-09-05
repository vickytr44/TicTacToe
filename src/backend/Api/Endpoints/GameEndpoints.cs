namespace TicTacToe.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.ValueObjects;
using TicTacToe.Infrastructure.Data;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games");

        group.MapPost("/", async (CreateGameRequest request, TicTacToeDbContext db) =>
        {
            var mode = Enum.TryParse<GameMode>(request.Mode, true, out var parsedMode)
                ? parsedMode
                : GameMode.TwoPlayer;

            var game = Game.Create(mode);
            db.Games.Add(game);
            await db.SaveChangesAsync();

            return Results.Created($"/api/games/{game.Id}", game.ToDto());
        });

        group.MapGet("/{id:guid}", async (Guid id, TicTacToeDbContext db) =>
        {
            var game = await db.Games.FindAsync(id);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }

            return Results.Ok(game.ToDto());
        });

        group.MapPost("/{id:guid}/moves", async (Guid id, [FromBody] MakeMoveRequest request, TicTacToeDbContext db) =>
        {
            if (request.Row < 1 || request.Row > 3 || request.Column < 1 || request.Column > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(request), "Row and Column must be between 1 and 3.");
            }

            if (!Enum.TryParse<Player>(request.Player, true, out var player))
            {
                throw new ArgumentException($"Invalid player mark '{request.Player}'. Must be 'X' or 'O'.");
            }

            var game = await db.Games.FindAsync(id);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }

            var position = CellPosition.FromOneBased(request.Row, request.Column);
            game.MakeMove(player, position);

            // If game just completed, update scoreboard exactly once
            if (game.Status == GameStatus.Won && game.Winner.HasValue)
            {
                var scoreboard = await db.Scoreboards.FirstOrDefaultAsync(s => s.Id == 1);
                if (scoreboard != null)
                {
                    scoreboard.RecordWin(game.Winner.Value);
                }
            }
            else if (game.Status == GameStatus.Draw)
            {
                var scoreboard = await db.Scoreboards.FirstOrDefaultAsync(s => s.Id == 1);
                if (scoreboard != null)
                {
                    scoreboard.RecordDraw();
                }
            }

            await db.SaveChangesAsync();

            return Results.Ok(game.ToDto());
        });
    }
}
