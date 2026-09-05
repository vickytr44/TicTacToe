namespace TicTacToe.Domain.Repositories;

using TicTacToe.Domain.Entities;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Game game, CancellationToken cancellationToken = default);
    Task UpdateAsync(Game game, CancellationToken cancellationToken = default);
}
