namespace TicTacToe.Domain.UnitTests;

using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Services;
using TicTacToe.Domain.ValueObjects;
using Xunit;

public class ComputerStrategyTests
{
    private static Player?[][] CreateEmptyBoard() =>
    [
        [null, null, null],
        [null, null, null],
        [null, null, null]
    ];

    [Fact]
    public void Priority1_Win_TakesWinningMove_Row()
    {
        // Arrange: O has (0,0) and (0,1)
        var board = CreateEmptyBoard();
        board[0][0] = Player.O;
        board[0][1] = Player.O;
        board[1][0] = Player.X;
        board[1][1] = Player.X; // X also threatening row 1

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O completes row 0 at (0,2)
        Assert.NotNull(move);
        Assert.Equal(0, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority1_Win_TakesWinningMove_Column()
    {
        // Arrange: O has (0,1) and (1,1)
        var board = CreateEmptyBoard();
        board[0][1] = Player.O;
        board[1][1] = Player.O;
        board[0][0] = Player.X;
        board[1][0] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O completes col 1 at (2,1)
        Assert.NotNull(move);
        Assert.Equal(2, move.Row);
        Assert.Equal(1, move.Column);
    }

    [Fact]
    public void Priority1_Win_TakesWinningMove_MainDiagonal()
    {
        // Arrange: O has (0,0) and (2,2)
        var board = CreateEmptyBoard();
        board[0][0] = Player.O;
        board[2][2] = Player.O;
        board[0][1] = Player.X;
        board[0][2] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O completes diagonal at (1,1)
        Assert.NotNull(move);
        Assert.Equal(1, move.Row);
        Assert.Equal(1, move.Column);
    }

    [Fact]
    public void Priority1_Win_TakesWinningMove_AntiDiagonal()
    {
        // Arrange: O has (0,2) and (1,1)
        var board = CreateEmptyBoard();
        board[0][2] = Player.O;
        board[1][1] = Player.O;
        board[0][0] = Player.X;
        board[1][0] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O completes anti-diagonal at (2,0)
        Assert.NotNull(move);
        Assert.Equal(2, move.Row);
        Assert.Equal(0, move.Column);
    }

    [Fact]
    public void Priority2_Block_BlocksOpponentWinningMove_Row()
    {
        // Arrange: X has (1,0) and (1,1), O has (0,0)
        var board = CreateEmptyBoard();
        board[1][0] = Player.X;
        board[1][1] = Player.X;
        board[0][0] = Player.O;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O blocks X at (1,2)
        Assert.NotNull(move);
        Assert.Equal(1, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority2_Block_BlocksOpponentWinningMove_Column()
    {
        // Arrange: X has (0,2) and (2,2), O has (1,1)
        var board = CreateEmptyBoard();
        board[0][2] = Player.X;
        board[2][2] = Player.X;
        board[1][1] = Player.O;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O blocks X at (1,2)
        Assert.NotNull(move);
        Assert.Equal(1, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority2_Block_BlocksOpponentWinningMove_Diagonal()
    {
        // Arrange: X has (0,0) and (1,1), O has (0,1)
        var board = CreateEmptyBoard();
        board[0][0] = Player.X;
        board[1][1] = Player.X;
        board[0][1] = Player.O;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O blocks X at (2,2)
        Assert.NotNull(move);
        Assert.Equal(2, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority1_PreferredOver_Priority2_WinTakesPrecedenceOverBlock()
    {
        // Arrange: O can win on row 0, X can win on row 2
        var board = CreateEmptyBoard();
        board[0][0] = Player.O;
        board[0][1] = Player.O;
        board[2][0] = Player.X;
        board[2][1] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O chooses to win at (0,2) instead of blocking at (2,2)
        Assert.NotNull(move);
        Assert.Equal(0, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority3_Center_TakesCenterIfAvailable()
    {
        // Arrange: X at (0,0), center (1,1) is free, no win or block possible
        var board = CreateEmptyBoard();
        board[0][0] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O takes center (1,1)
        Assert.NotNull(move);
        Assert.Equal(1, move.Row);
        Assert.Equal(1, move.Column);
    }

    [Fact]
    public void Priority4_Corner_TakesFirstAvailableCorner_InFixedOrder()
    {
        // Arrange: Center (1,1) is occupied by X, no win or block possible.
        // Corner order: (0,0), (0,2), (2,0), (2,2)
        var board = CreateEmptyBoard();
        board[1][1] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O takes first corner (0,0)
        Assert.NotNull(move);
        Assert.Equal(0, move.Row);
        Assert.Equal(0, move.Column);
    }

    [Fact]
    public void Priority4_Corner_TakesNextCornerIfFirstOccupied()
    {
        // Arrange: (0,0) and (1,1) occupied, no win or block
        var board = CreateEmptyBoard();
        board[0][0] = Player.X;
        board[1][1] = Player.O;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: Next corner is (0,2)
        Assert.NotNull(move);
        Assert.Equal(0, move.Row);
        Assert.Equal(2, move.Column);
    }

    [Fact]
    public void Priority5_Any_TakesFirstRemainingCell_WhenCenterAndCornersOccupied()
    {
        // Arrange: Center and all 4 corners occupied, no win or block possible
        var board = CreateEmptyBoard();
        board[0][0] = Player.X;
        board[0][2] = Player.O;
        board[2][0] = Player.O;
        board[2][2] = Player.X;
        board[1][1] = Player.X;

        // Act: Remaining cells are edges (0,1), (1,0), (1,2), (2,1)
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert: O takes first available remaining cell (0,1)
        Assert.NotNull(move);
        Assert.Equal(0, move.Row);
        Assert.Equal(1, move.Column);
    }

    [Fact]
    public void FullBoard_ReturnsNull()
    {
        // Arrange: Full board
        var board = CreateEmptyBoard();
        board[0][0] = Player.X; board[0][1] = Player.O; board[0][2] = Player.X;
        board[1][0] = Player.X; board[1][1] = Player.O; board[1][2] = Player.O;
        board[2][0] = Player.O; board[2][1] = Player.X; board[2][2] = Player.X;

        // Act
        var move = ComputerStrategy.CalculateMove(board, Player.O);

        // Assert
        Assert.Null(move);
    }
}
