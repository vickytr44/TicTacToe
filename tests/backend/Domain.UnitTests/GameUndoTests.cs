namespace TicTacToe.Domain.UnitTests;

using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.ValueObjects;

public class GameUndoTests
{
    [Fact]
    public void UndoMove_SingleMove_RemovesMoveAndRevertsTurnToX()
    {
        var game = Game.Create();
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));

        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);

        game.UndoMove();

        Assert.Null(game.Board[0][0]);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Empty(game.Moves);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void UndoMove_TwoMoves_RemovesLastMoveAndRevertsTurnToO()
    {
        var game = Game.Create();
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));

        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(2, game.Moves.Count);

        game.UndoMove();

        Assert.Equal(Player.X, game.Board[0][0]);
        Assert.Null(game.Board[1][1]);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);
        Assert.Equal(Player.X, game.Moves[0].Player);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void UndoMove_MultipleConsecutiveUndos_RevertsAllTheWayToEmptyBoard()
    {
        var game = Game.Create();
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));

        Assert.Equal(3, game.Moves.Count);

        // First undo: removes move 3 (X at 2,2)
        game.UndoMove();
        Assert.Equal(2, game.Moves.Count);
        Assert.Null(game.Board[2][2]);
        Assert.Equal(Player.X, game.CurrentPlayer);

        // Second undo: removes move 2 (O at 1,1)
        game.UndoMove();
        Assert.Single(game.Moves);
        Assert.Null(game.Board[1][1]);
        Assert.Equal(Player.O, game.CurrentPlayer);

        // Third undo: removes move 1 (X at 0,0)
        game.UndoMove();
        Assert.Empty(game.Moves);
        Assert.Null(game.Board[0][0]);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void UndoMove_NoMoves_ThrowsInvalidOperationException()
    {
        var game = Game.Create();

        var ex = Assert.Throws<InvalidOperationException>(() => game.UndoMove());
        Assert.Contains("No moves to undo", ex.Message);
    }

    [Fact]
    public void UndoMove_AfterWin_ThrowsInvalidOperationException_OptionA()
    {
        var game = Game.Create();
        // Row 0 win for X
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2));

        Assert.Equal(GameStatus.Won, game.Status);

        var ex = Assert.Throws<InvalidOperationException>(() => game.UndoMove());
        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UndoMove_AfterDraw_ThrowsInvalidOperationException_OptionA()
    {
        var game = Game.Create();
        // Draw sequence:
        // (0,0)[X], (0,1)[O], (0,2)[X]
        // (1,0)[X], (1,1)[O], (1,2)[O]
        // (2,0)[O], (2,1)[X], (2,2)[X]
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(0, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 2));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 1));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(1, 0));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(1, 2));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 1));
        game.MakeMove(Player.O, CellPosition.FromZeroBased(2, 0));
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));

        Assert.Equal(GameStatus.Draw, game.Status);

        var ex = Assert.Throws<InvalidOperationException>(() => game.UndoMove());
        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UndoMove_ComputerMode_PopsBothComputerAndHumanMoves()
    {
        var game = Game.Create(GameMode.Computer);
        // Human plays (0,0) -> Computer automatically plays center (1,1)
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));

        Assert.Equal(2, game.Moves.Count);
        Assert.Equal(Player.X, game.Board[0][0]);
        Assert.Equal(Player.O, game.Board[1][1]);
        Assert.Equal(Player.X, game.CurrentPlayer);

        // Undo move pair
        game.UndoMove();

        Assert.Empty(game.Moves);
        Assert.Null(game.Board[0][0]);
        Assert.Null(game.Board[1][1]);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void UndoMove_ComputerMode_MultipleConsecutiveUndos_PopsMovePairsUntilEmpty()
    {
        var game = Game.Create(GameMode.Computer);

        // Round 1: X (0,0) -> O (1,1)
        game.MakeMove(Player.X, CellPosition.FromZeroBased(0, 0));

        // Round 2: X (2,2) -> O (0,2)
        game.MakeMove(Player.X, CellPosition.FromZeroBased(2, 2));

        Assert.Equal(4, game.Moves.Count);

        // First Undo: pops round 2 (O at 0,2 and X at 2,2)
        game.UndoMove();

        Assert.Equal(2, game.Moves.Count);
        Assert.Equal(Player.X, game.Board[0][0]);
        Assert.Equal(Player.O, game.Board[1][1]);
        Assert.Null(game.Board[2][2]);
        Assert.Null(game.Board[0][2]);
        Assert.Equal(Player.X, game.CurrentPlayer);

        // Second Undo: pops round 1 (O at 1,1 and X at 0,0)
        game.UndoMove();

        Assert.Empty(game.Moves);
        Assert.Null(game.Board[0][0]);
        Assert.Null(game.Board[1][1]);
        Assert.Equal(Player.X, game.CurrentPlayer);

        // Third Undo: throws when empty
        var ex = Assert.Throws<InvalidOperationException>(() => game.UndoMove());
        Assert.Contains("No moves to undo", ex.Message);
    }

    [Fact]
    public void UndoMove_ComputerMode_NoMoves_ThrowsInvalidOperationException()
    {
        var game = Game.Create(GameMode.Computer);

        var ex = Assert.Throws<InvalidOperationException>(() => game.UndoMove());
        Assert.Contains("No moves to undo", ex.Message);
    }
}
