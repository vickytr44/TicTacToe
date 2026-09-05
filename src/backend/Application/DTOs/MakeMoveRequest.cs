namespace TicTacToe.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public sealed record MakeMoveRequest
{
    [Required]
    public string Player { get; init; } = string.Empty;

    [Range(1, 3)]
    public int Row { get; init; }

    [Range(1, 3)]
    public int Column { get; init; }
}
