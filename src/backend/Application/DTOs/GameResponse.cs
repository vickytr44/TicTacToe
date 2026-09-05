namespace TicTacToe.Application.DTOs;

public sealed record GameResponse(
    Guid Id,
    string?[][] Board,
    string CurrentPlayer,
    string GameMode,
    string Status,
    string? Winner,
    IReadOnlyList<CellPositionDto> WinningCells,
    IReadOnlyList<MoveDto> Moves,
    DateTimeOffset CreatedAt
);
