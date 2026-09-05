namespace TicTacToe.Domain.Repositories;

using TicTacToe.Domain.Entities;

public interface IScoreboardRepository
{
    Task<Scoreboard> GetScoreboardAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Scoreboard scoreboard, CancellationToken cancellationToken = default);
    Task ResetAsync(CancellationToken cancellationToken = default);
}
