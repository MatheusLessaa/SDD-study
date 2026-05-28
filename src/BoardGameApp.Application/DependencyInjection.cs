using BoardGameApp.Application.Authors;
using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
    }
}
