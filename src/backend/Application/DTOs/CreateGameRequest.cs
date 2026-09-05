namespace TicTacToe.Application.DTOs;

public sealed record CreateGameRequest
{
    public string Mode { get; init; } = "TwoPlayer";
}
