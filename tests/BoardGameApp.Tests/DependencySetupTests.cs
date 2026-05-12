using BoardGameApp.Application;
using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Common;
using BoardGameApp.Domain.Games;
using BoardGameApp.Domain.Genres;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Domain.Players;
using BoardGameApp.Domain.Publishers;
using BoardGameApp.Infrastructure;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameApp.Tests;

public class DependencySetupTests
{
    [Fact]
    public void Test_project_can_reference_domain_and_ef_core()
    {
        Assert.Equal("BoardGameApp.Domain", typeof(Entity).Assembly.GetName().Name);
        Assert.Equal("Microsoft.EntityFrameworkCore", typeof(DbContext).Namespace);
    }

    [Fact]
    public void Infrastructure_registers_app_db_context_with_sql_server_provider()
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BoardGameAppTests;Trusted_Connection=True;TrustServerCertificate=True";
        var services = new ServiceCollection();

        services.AddInfrastructure(connectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", dbContext.Database.ProviderName);
    }

    [Fact]
    public void Infrastructure_registers_player_repository()
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BoardGameAppTests;Trusted_Connection=True;TrustServerCertificate=True";
        var services = new ServiceCollection();

        services.AddInfrastructure(connectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();

        Assert.NotNull(repository);
    }

    [Fact]
    public void Infrastructure_registers_game_repository()
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BoardGameAppTests;Trusted_Connection=True;TrustServerCertificate=True";
        var services = new ServiceCollection();

        services.AddInfrastructure(connectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        Assert.NotNull(repository);
    }

    [Fact]
    public void Infrastructure_registers_match_repository()
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BoardGameAppTests;Trusted_Connection=True;TrustServerCertificate=True";
        var services = new ServiceCollection();

        services.AddInfrastructure(connectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();

        Assert.NotNull(repository);
    }

    [Fact]
    public void Application_and_infrastructure_register_all_services_and_repositories()
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BoardGameAppTests;Trusted_Connection=True;TrustServerCertificate=True";
        var services = new ServiceCollection();

        services.AddApplication();
        services.AddInfrastructure(connectionString);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPlayerService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IGameService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMatchService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPlayerRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IGameRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMatchRepository>());
    }

    [Fact]
    public void App_db_context_exposes_all_required_db_sets()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("db-set-access")
            .Options;

        using var dbContext = new AppDbContext(options);

        Assert.NotNull(dbContext.Players);
        Assert.NotNull(dbContext.Games);
        Assert.NotNull(dbContext.Matches);
        Assert.NotNull(dbContext.Genres);
        Assert.NotNull(dbContext.Publishers);
    }

    [Fact]
    public void App_db_context_model_contains_all_domain_entities()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("model-entity-check")
            .Options;

        using var dbContext = new AppDbContext(options);

        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Player)));
        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Game)));
        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Match)));
        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Genre)));
        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Publisher)));
    }

    [Fact]
    public void App_db_context_configures_required_unique_indexes()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("constraint-check")
            .Options;

        using var dbContext = new AppDbContext(options);
        var player = dbContext.Model.FindEntityType(typeof(Player));
        var game = dbContext.Model.FindEntityType(typeof(Game));

        Assert.NotNull(player);
        Assert.NotNull(game);

        Assert.Contains(
            player.GetIndexes(),
            index => index.IsUnique
                && index.Properties.Select(property => property.Name).SequenceEqual(["FullName"]));

        Assert.Contains(
            player.GetIndexes(),
            index => index.IsUnique
                && index.Properties.Select(property => property.Name).SequenceEqual(["WhatsApp"]));

        Assert.Contains(
            game.GetIndexes(),
            index => index.IsUnique
                && index.Properties.Select(property => property.Name).SequenceEqual(["Name", "PublisherId"]));
    }

    [Fact]
    public void App_db_context_configures_required_foreign_keys()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("foreign-key-check")
            .Options;

        using var dbContext = new AppDbContext(options);
        var game = dbContext.Model.FindEntityType(typeof(Game));
        var match = dbContext.Model.FindEntityType(typeof(Match));

        Assert.NotNull(game);
        Assert.NotNull(match);

        Assert.Contains(
            game.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Publisher)
                && key.Properties.Select(property => property.Name).SequenceEqual(["PublisherId"]));

        Assert.Contains(
            game.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Genre)
                && key.Properties.Select(property => property.Name).SequenceEqual(["GenreId"]));

        Assert.Contains(
            match.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Game)
                && key.Properties.Select(property => property.Name).SequenceEqual(["GameId"]));

        Assert.Contains(
            match.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Player)
                && key.Properties.Select(property => property.Name).SequenceEqual(["WinnerPlayerId"]));
    }

    [Fact]
    public void App_db_context_configures_initial_seed_data()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("seed-data-check")
            .Options;

        using var dbContext = new AppDbContext(options);
        var designTimeModel = dbContext.GetService<IDesignTimeModel>().Model;
        var genre = designTimeModel.FindEntityType(typeof(Genre));
        var publisher = designTimeModel.FindEntityType(typeof(Publisher));

        Assert.NotNull(genre);
        Assert.NotNull(publisher);

        Assert.Equal(3, genre.GetSeedData().Count());
        Assert.Equal(3, publisher.GetSeedData().Count());
        Assert.Contains(genre.GetSeedData(), seed => seed["Name"]?.ToString() == "Strategy");
        Assert.Contains(publisher.GetSeedData(), seed => seed["Name"]?.ToString() == "Galapagos");
    }
}
