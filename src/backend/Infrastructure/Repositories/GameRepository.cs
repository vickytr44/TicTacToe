namespace TicTacToe.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Repositories;
using TicTacToe.Infrastructure.Data;

public class GameRepository(TicTacToeDbContext context) : IGameRepository
{
    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Games.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
    {
        await context.Games.AddAsync(game, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        context.Games.Update(game);
        await context.SaveChangesAsync(cancellationToken);
    }
}
