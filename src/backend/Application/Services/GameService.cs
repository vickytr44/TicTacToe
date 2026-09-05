namespace TicTacToe.Application.Services;

using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Repositories;
using TicTacToe.Domain.ValueObjects;

public class GameService(
    IGameRepository gameRepository,
    IScoreboardRepository scoreboardRepository) : IGameService
{
    public async Task<GameResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default)
    {
        var mode = Enum.TryParse<GameMode>(request.Mode, true, out var parsedMode)
            ? parsedMode
            : GameMode.TwoPlayer;

        var game = Game.Create(mode);
        await gameRepository.AddAsync(game, cancellationToken);

        return game.ToDto();
    }

    public async Task<GameResponse> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        return game.ToDto();
    }

    public async Task<GameResponse> MakeMoveAsync(Guid id, MakeMoveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Row < 1 || request.Row > 3 || request.Column < 1 || request.Column > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Row and Column must be between 1 and 3.");
        }

        if (!Enum.TryParse<Player>(request.Player, true, out var player))
        {
            throw new ArgumentException($"Invalid player mark '{request.Player}'. Must be 'X' or 'O'.");
        }

        var game = await gameRepository.GetByIdAsync(id, cancellationToken);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        var position = CellPosition.FromOneBased(request.Row, request.Column);
        game.MakeMove(player, position);

        if (game.Status == GameStatus.Won && game.Winner.HasValue)
        {
            var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
            scoreboard.RecordWin(game.Winner.Value);
            await scoreboardRepository.UpdateAsync(scoreboard, cancellationToken);
        }
        else if (game.Status == GameStatus.Draw)
        {
            var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
            scoreboard.RecordDraw();
            await scoreboardRepository.UpdateAsync(scoreboard, cancellationToken);
        }

        await gameRepository.UpdateAsync(game, cancellationToken);

        return game.ToDto();
    }

    public async Task<GameResponse> ResetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        game.Reset();
        await gameRepository.UpdateAsync(game, cancellationToken);

        return game.ToDto();
    }

    public async Task<GameResponse> UndoMoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        game.UndoMove();
        await gameRepository.UpdateAsync(game, cancellationToken);

        return game.ToDto();
    }
}
