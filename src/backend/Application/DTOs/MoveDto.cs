namespace TicTacToe.Application.DTOs;

public sealed record MoveDto(int MoveNumber, string Player, int Row, int Column);
