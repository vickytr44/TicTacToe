namespace TicTacToe.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Domain.Repositories;
using TicTacToe.Infrastructure.Data;
using TicTacToe.Infrastructure.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tictactoe.db";

        services.AddDbContext<TicTacToeDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IScoreboardRepository, ScoreboardRepository>();

        return services;
    }
}
