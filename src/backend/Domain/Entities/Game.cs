namespace TicTacToe.Domain.Entities;

using TicTacToe.Domain.Enums;
using TicTacToe.Domain.ValueObjects;

public class Game
{
    public Guid Id { get; private set; }
    public Player?[][] Board { get; private set; } = [];
    public Player CurrentPlayer { get; private set; }
    public GameMode GameMode { get; private set; }
    public GameStatus Status { get; private set; }
    public Player? Winner { get; private set; }
    public List<CellPosition> WinningCells { get; private set; } = [];
    public List<Move> Moves { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }

    public Game()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        GameMode = GameMode.TwoPlayer;
        InitBoard();
    }

    public Game(Guid id, GameMode gameMode, DateTimeOffset createdAt)
    {
        Id = id;
        GameMode = gameMode;
        CreatedAt = createdAt;
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        InitBoard();
    }

    public static Game Create(GameMode gameMode = GameMode.TwoPlayer)
    {
        return new Game(Guid.NewGuid(), gameMode, DateTimeOffset.UtcNow);
    }

    public void Reset()
    {
        InitBoard();
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];
        Moves = [];
    }

    private void InitBoard()
    {
        Board =
        [
            [null, null, null],
            [null, null, null],
            [null, null, null]
        ];
    }
}
