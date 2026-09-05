namespace TicTacToe.Domain.Services;

using TicTacToe.Domain.Enums;
using TicTacToe.Domain.ValueObjects;

public static class ComputerStrategy
{
    private static readonly (int Row, int Col)[][] WinningLines =
    [
        // Rows
        [(0, 0), (0, 1), (0, 2)],
        [(1, 0), (1, 1), (1, 2)],
        [(2, 0), (2, 1), (2, 2)],
        // Columns
        [(0, 0), (1, 0), (2, 0)],
        [(0, 1), (1, 1), (2, 1)],
        [(0, 2), (1, 2), (2, 2)],
        // Main diagonal
        [(0, 0), (1, 1), (2, 2)],
        // Anti-diagonal
        [(0, 2), (1, 1), (2, 0)]
    ];

    private static readonly (int Row, int Col)[] Corners =
    [
        (0, 0),
        (0, 2),
        (2, 0),
        (2, 2)
    ];

    public static CellPosition? CalculateMove(Player?[][] board, Player computerPlayer = Player.O)
    {
        var opponent = computerPlayer == Player.X ? Player.O : Player.X;

        // Priority 1: Win if possible
        var winningMove = FindWinningMoveFor(board, computerPlayer);
        if (winningMove != null)
        {
            return winningMove;
        }

        // Priority 2: Block opponent's win
        var blockingMove = FindWinningMoveFor(board, opponent);
        if (blockingMove != null)
        {
            return blockingMove;
        }

        // Priority 3: Take center if available (1,1)
        if (board[1][1] == null)
        {
            return CellPosition.FromZeroBased(1, 1);
        }

        // Priority 4: Take corner in fixed order: (0,0), (0,2), (2,0), (2,2)
        foreach (var (r, c) in Corners)
        {
            if (board[r][c] == null)
            {
                return CellPosition.FromZeroBased(r, c);
            }
        }

        // Priority 5: Take any available cell (row-major order)
        for (var r = 0; r < 3; r++)
        {
            for (var c = 0; c < 3; c++)
            {
                if (board[r][c] == null)
                {
                    return CellPosition.FromZeroBased(r, c);
                }
            }
        }

        return null;
    }

    private static CellPosition? FindWinningMoveFor(Player?[][] board, Player player)
    {
        foreach (var line in WinningLines)
        {
            var playerCount = 0;
            var nullCount = 0;
            (int Row, int Col)? emptyCell = null;

            foreach (var (r, c) in line)
            {
                if (board[r][c] == player)
                {
                    playerCount++;
                }
                else if (board[r][c] == null)
                {
                    nullCount++;
                    emptyCell = (r, c);
                }
            }

            if (playerCount == 2 && nullCount == 1 && emptyCell.HasValue)
            {
                return CellPosition.FromZeroBased(emptyCell.Value.Row, emptyCell.Value.Col);
            }
        }

        return null;
    }
}
