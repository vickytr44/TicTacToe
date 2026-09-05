namespace TicTacToe.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Repositories;
using TicTacToe.Domain.Entities;
using TicTacToe.Infrastructure.Data;

public class ScoreboardRepository(TicTacToeDbContext context) : IScoreboardRepository
{
    public async Task<Scoreboard> GetScoreboardAsync(CancellationToken cancellationToken = default)
    {
        var scoreboard = await context.Scoreboards.FirstOrDefaultAsync(s => s.Id == 1, cancellationToken);
        if (scoreboard == null)
        {
            scoreboard = new Scoreboard { Id = 1 };
            await context.Scoreboards.AddAsync(scoreboard, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return scoreboard;
    }

    public async Task UpdateAsync(Scoreboard scoreboard, CancellationToken cancellationToken = default)
    {
        context.Scoreboards.Update(scoreboard);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        var scoreboard = await GetScoreboardAsync(cancellationToken);
        scoreboard.Reset();
        await context.SaveChangesAsync(cancellationToken);
    }
}
