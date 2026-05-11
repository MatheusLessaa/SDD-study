using BoardGameApp.Domain.Common;
using BoardGameApp.Domain.Games;
using BoardGameApp.Domain.Genres;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Domain.Players;
using BoardGameApp.Domain.Publishers;
using BoardGameApp.Infrastructure;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
}
