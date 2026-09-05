namespace TicTacToe.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tictactoe.db";

        services.AddDbContext<TicTacToeDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
