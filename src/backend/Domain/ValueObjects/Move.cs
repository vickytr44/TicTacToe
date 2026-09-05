namespace TicTacToe.Domain.ValueObjects;

using TicTacToe.Domain.Enums;

public sealed record Move(int MoveNumber, Player Player, CellPosition Position);
