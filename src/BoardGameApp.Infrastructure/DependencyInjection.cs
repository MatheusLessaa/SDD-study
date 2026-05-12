using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using BoardGameApp.Infrastructure.Games;
using BoardGameApp.Infrastructure.Matches;
using BoardGameApp.Infrastructure.Persistence;
using BoardGameApp.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        return services;
    }
}
