namespace TicTacToe.Application;

using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        return services;
    }
}
