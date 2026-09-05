namespace TicTacToe.Domain.ValueObjects;

public sealed record CellPosition(int Row, int Column)
{
    public static CellPosition FromZeroBased(int row, int col)
    {
        if (row is < 0 or > 2 || col is < 0 or > 2)
        {
            throw new ArgumentOutOfRangeException($"Coordinates ({row}, {col}) are out of 0-based bounds [0..2].");
        }
        return new CellPosition(row, col);
    }

    public static CellPosition FromOneBased(int row, int col)
    {
        if (row is < 1 or > 3 || col is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException($"Coordinates ({row}, {col}) are out of 1-based bounds [1..3].");
        }
        return new CellPosition(row - 1, col - 1);
    }

    public int OneBasedRow => Row + 1;
    public int OneBasedColumn => Column + 1;
}
