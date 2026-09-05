namespace TicTacToe.Application.DTOs;

using TicTacToe.Domain.Entities;

public static class GameMappings
{
    public static GameResponse ToDto(this Game game)
    {
        var boardDto = new string?[3][];
        for (var r = 0; r < 3; r++)
        {
            boardDto[r] = new string?[3];
            for (var c = 0; c < 3; c++)
            {
                boardDto[r][c] = game.Board[r][c]?.ToString();
            }
        }

        var winningCells = game.WinningCells
            .Select(c => new CellPositionDto(c.OneBasedRow, c.OneBasedColumn))
            .ToList();

        var moves = game.Moves
            .Select(m => new MoveDto(m.MoveNumber, m.Player.ToString(), m.Position.OneBasedRow, m.Position.OneBasedColumn))
            .ToList();

        return new GameResponse(
            game.Id,
            boardDto,
            game.CurrentPlayer.ToString(),
            game.GameMode.ToString(),
            game.Status.ToString(),
            game.Winner?.ToString(),
            winningCells,
            moves,
            game.CreatedAt
        );
    }

    public static ScoreboardResponse ToDto(this Scoreboard scoreboard)
    {
        return new ScoreboardResponse(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
    }
}
