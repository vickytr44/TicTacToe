namespace TicTacToe.Domain.UnitTests;

using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.ValueObjects;
using Xunit;

public class ScoreboardTests
{
    [Fact]
    public void Scoreboard_DefaultState_HasZeroCounts()
    {
        // Act
        var scoreboard = new Scoreboard();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordWin_PlayerX_IncrementsXWinsByOne()
    {
        // Arrange
        var scoreboard = new Scoreboard();

        // Act
        scoreboard.RecordWin(Player.X);

        // Assert
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordWin_PlayerO_IncrementsOWinsByOne()
    {
        // Arrange
        var scoreboard = new Scoreboard();

        // Act
        scoreboard.RecordWin(Player.O);

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordDraw_IncrementsDrawsByOne()
    {
        // Arrange
        var scoreboard = new Scoreboard();

        // Act
        scoreboard.RecordDraw();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void Reset_ClearsAllCountsToZero()
    {
        // Arrange
        var scoreboard = new Scoreboard(xWins: 4, oWins: 2, draws: 1);

        // Act
        scoreboard.Reset();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordWin_CalledMultipleTimes_IncrementsExactOncePerCall()
    {
        // Arrange
        var scoreboard = new Scoreboard();

        // Act
        scoreboard.RecordWin(Player.X);
        scoreboard.RecordWin(Player.X);
        scoreboard.RecordWin(Player.O);
        scoreboard.RecordDraw();

        // Assert
        Assert.Equal(2, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void IncompleteGame_DoesNotTriggerScoreboardIncrement()
    {
        // Arrange
        var game = Game.Create(GameMode.TwoPlayer);
        var scoreboard = new Scoreboard();

        // Act: Make a non-terminal move
        game.MakeMove(Player.X, CellPosition.FromOneBased(1, 1));

        // Assert: Game is InProgress, scoreboard should not be incremented
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void GameReset_PreservesScoreboardCounts()
    {
        // Arrange
        var scoreboard = new Scoreboard(xWins: 3, oWins: 2, draws: 1);
        var game = Game.Create(GameMode.TwoPlayer);

        // Act
        game.Reset();

        // Assert: Scoreboard retains its existing session totals
        Assert.Equal(3, scoreboard.XWins);
        Assert.Equal(2, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }
}
