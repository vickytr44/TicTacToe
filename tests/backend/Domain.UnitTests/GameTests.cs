namespace TicTacToe.Domain.UnitTests;

using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.ValueObjects;

public class GameTests
{
    [Fact]
    public void NewGame_HasEmptyBoardAndPlayerXTurn()
    {
        var game = Game.Create();

        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
        Assert.Empty(game.Moves);

        for (var r = 0; r < 3; r++)
        {
            for (var c = 0; c < 3; c++)
            {
                Assert.Null(game.Board[r][c]);
            }
        }
    }

    [Fact]
    public void MakeMove_ValidMove_PlacesMarkAndSwitchesPlayer()
    {
        var game = Game.Create();

        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));

        Assert.Equal(Player.X, game.Board[0][0]);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);
        Assert.Equal(1, game.Moves[0].MoveNumber);
        Assert.Equal(Player.X, game.Moves[0].Player);
        Assert.Equal(0, game.Moves[0].Position.Row);
        Assert.Equal(0, game.Moves[0].Position.Column);
    }

    [Fact]
    public void MakeMove_OccupiedCell_ThrowsInvalidMoveException()
    {
        var game = Game.Create();
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 1));

        var ex = Assert.Throws<InvalidMoveException>(() =>
            game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1)));

        Assert.Contains("occupied", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_WrongPlayer_ThrowsInvalidMoveException()
    {
        var game = Game.Create();

        var ex = Assert.Throws<InvalidMoveException>(() =>
            game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 0)));

        Assert.Contains("not Player O's turn", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_RowWin_DetectsWinAndSetsWinningCells()
    {
        var game = Game.Create();
        // Row 0: (0,0)[X], (1,0)[O], (0,1)[X], (1,1)[O], (0,2)[X] -> X wins Row 0
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);

        var expectedWinningCells = new[]
        {
            CellPosition.FromZeroBased(0, 0),
            CellPosition.FromZeroBased(0, 1),
            CellPosition.FromZeroBased(0, 2)
        };
        Assert.All(expectedWinningCells, cell => Assert.Contains(cell, game.WinningCells));
    }

    [Fact]
    public void MakeMove_ColumnWin_DetectsWinAndSetsWinningCells()
    {
        var game = Game.Create();
        // Col 1: (0,0)[X], (0,1)[O], (1,0)[X], (1,1)[O], (2,2)[X], (2,1)[O] -> O wins Col 1
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 1));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.O, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);

        var expectedWinningCells = new[]
        {
            CellPosition.FromZeroBased(0, 1),
            CellPosition.FromZeroBased(1, 1),
            CellPosition.FromZeroBased(2, 1)
        };
        Assert.All(expectedWinningCells, cell => Assert.Contains(cell, game.WinningCells));
    }

    [Fact]
    public void MakeMove_MainDiagonalWin_DetectsWinAndSetsWinningCells()
    {
        var game = Game.Create();
        // Diag: (0,0)[X], (0,1)[O], (1,1)[X], (0,2)[O], (2,2)[X] -> X wins Main Diag
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 2));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);

        var expectedWinningCells = new[]
        {
            CellPosition.FromZeroBased(0, 0),
            CellPosition.FromZeroBased(1, 1),
            CellPosition.FromZeroBased(2, 2)
        };
        Assert.All(expectedWinningCells, cell => Assert.Contains(cell, game.WinningCells));
    }

    [Fact]
    public void MakeMove_AntiDiagonalWin_DetectsWinAndSetsWinningCells()
    {
        var game = Game.Create();
        // Anti-Diag: (0,0)[X], (0,2)[O], (1,0)[X], (1,1)[O], (2,2)[X], (2,0)[O] -> O wins
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 2));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 0));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.O, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);

        var expectedWinningCells = new[]
        {
            CellPosition.FromZeroBased(0, 2),
            CellPosition.FromZeroBased(1, 1),
            CellPosition.FromZeroBased(2, 0)
        };
        Assert.All(expectedWinningCells, cell => Assert.Contains(cell, game.WinningCells));
    }

    [Fact]
    public void MakeMove_AfterGameWon_ThrowsInvalidMoveException()
    {
        var game = Game.Create();
        // X wins row 0
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2));

        var ex = Assert.Throws<InvalidMoveException>(() =>
            game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 2)));

        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_DrawGame_DetectsDrawAndSetsStatus()
    {
        var game = Game.Create();
        // Sequence producing Draw:
        // Row 0: (0,0)[X], (0,1)[O], (0,2)[X]
        // Row 1: (1,0)[X], (1,1)[O], (1,2)[O]
        // Row 2: (2,0)[O], (2,1)[X], (2,2)[X]
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0)); // 1. X
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 1)); // 2. O
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2)); // 3. X
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1)); // 4. O
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 0)); // 5. X
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 2)); // 6. O
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 1)); // 7. X
        game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 0)); // 8. O
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2)); // 9. X

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
        Assert.Equal(9, game.Moves.Count);
    }

    [Fact]
    public void MakeMove_AfterDraw_ThrowsInvalidMoveException()
    {
        var game = Game.Create();
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 2));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 0));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));

        var ex = Assert.Throws<InvalidMoveException>(() =>
            game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 0)));

        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
