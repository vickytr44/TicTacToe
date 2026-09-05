namespace TicTacToe.Domain.Entities;

using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.ValueObjects;

using TicTacToe.Domain.Services;

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

        if (GameMode == GameMode.Computer && player == Player.O)
        {
            throw new InvalidMoveException("Player O is controlled by the computer in Computer mode.");
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

        if (GameMode == GameMode.Computer && CurrentPlayer == Player.O && Status == GameStatus.InProgress)
        {
            ApplyComputerMove();
        }
    }

    private void ApplyComputerMove()
    {
        var computerMove = ComputerStrategy.CalculateMove(Board, Player.O);
        if (computerMove == null)
        {
            return;
        }

        var r = computerMove.Row;
        var c = computerMove.Column;

        Board[r][c] = Player.O;
        Moves.Add(new Move(Moves.Count + 1, Player.O, computerMove));

        if (CheckWin(Player.O, out var winningLine))
        {
            Status = GameStatus.Won;
            Winner = Player.O;
            WinningCells = winningLine;
            return;
        }

        if (Moves.Count == 9)
        {
            Status = GameStatus.Draw;
            return;
        }

        CurrentPlayer = Player.X;
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

    public void UndoMove()
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot undo move after the game has completed.");
        }

        if (Moves.Count == 0)
        {
            throw new InvalidOperationException("No moves to undo.");
        }

        if (GameMode == GameMode.Computer)
        {
            // Pop computer move (O)
            var computerMove = Moves[^1];
            Moves.RemoveAt(Moves.Count - 1);
            Board[computerMove.Position.Row][computerMove.Position.Column] = null;

            // Pop human move (X)
            if (Moves.Count > 0)
            {
                var humanMove = Moves[^1];
                Moves.RemoveAt(Moves.Count - 1);
                Board[humanMove.Position.Row][humanMove.Position.Column] = null;
            }

            CurrentPlayer = Player.X;
            Winner = null;
            WinningCells = [];
            Status = GameStatus.InProgress;
            return;
        }

        var lastMove = Moves[^1];
        Moves.RemoveAt(Moves.Count - 1);
        Board[lastMove.Position.Row][lastMove.Position.Column] = null;
        CurrentPlayer = lastMove.Player;
        Winner = null;
        WinningCells = [];
        Status = GameStatus.InProgress;
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
