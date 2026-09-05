namespace TicTacToe.Domain.Entities;

using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
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

    public void MakeMove(Player player, CellPosition position)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidMoveException("Game is already completed.");
        }

        if (player != CurrentPlayer)
        {
            throw new InvalidMoveException($"It is not Player {player}'s turn.");
        }

        var r = position.Row;
        var c = position.Column;

        if (Board[r][c] != null)
        {
            throw new InvalidMoveException($"Cell ({position.OneBasedRow},{position.OneBasedColumn}) is already occupied.");
        }

        Board[r][c] = player;
        Moves.Add(new Move(Moves.Count + 1, player, position));

        if (CheckWin(player, out var winningLine))
        {
            Status = GameStatus.Won;
            Winner = player;
            WinningCells = winningLine;
            return;
        }

        if (Moves.Count == 9)
        {
            Status = GameStatus.Draw;
            return;
        }

        CurrentPlayer = (CurrentPlayer == Player.X) ? Player.O : Player.X;
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

    private bool CheckWin(Player player, out List<CellPosition> winningLine)
    {
        // Check rows
        for (var row = 0; row < 3; row++)
        {
            if (Board[row][0] == player && Board[row][1] == player && Board[row][2] == player)
            {
                winningLine =
                [
                    CellPosition.FromZeroBased(row, 0),
                    CellPosition.FromZeroBased(row, 1),
                    CellPosition.FromZeroBased(row, 2)
                ];
                return true;
            }
        }

        // Check columns
        for (var col = 0; col < 3; col++)
        {
            if (Board[0][col] == player && Board[1][col] == player && Board[2][col] == player)
            {
                winningLine =
                [
                    CellPosition.FromZeroBased(0, col),
                    CellPosition.FromZeroBased(1, col),
                    CellPosition.FromZeroBased(2, col)
                ];
                return true;
            }
        }

        // Check main diagonal (top-left to bottom-right)
        if (Board[0][0] == player && Board[1][1] == player && Board[2][2] == player)
        {
            winningLine =
            [
                CellPosition.FromZeroBased(0, 0),
                CellPosition.FromZeroBased(1, 1),
                CellPosition.FromZeroBased(2, 2)
            ];
            return true;
        }

        // Check anti-diagonal (top-right to bottom-left)
        if (Board[0][2] == player && Board[1][1] == player && Board[2][0] == player)
        {
            winningLine =
            [
                CellPosition.FromZeroBased(0, 2),
                CellPosition.FromZeroBased(1, 1),
                CellPosition.FromZeroBased(2, 0)
            ];
            return true;
        }

        winningLine = [];
        return false;
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
