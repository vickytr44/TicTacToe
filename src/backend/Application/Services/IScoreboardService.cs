namespace TicTacToe.Application.Services;

using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Enums;

public interface IScoreboardService
{
    Task<ScoreboardResponse> GetScoreboardAsync(CancellationToken cancellationToken = default);
    Task<ScoreboardResponse> ResetScoreboardAsync(CancellationToken cancellationToken = default);
    Task RecordWinAsync(Player winner, CancellationToken cancellationToken = default);
    Task RecordDrawAsync(CancellationToken cancellationToken = default);
}
