using BoardGameApp.Application.Games;
using BoardGameApp.Domain.Games;
using BoardGameApp.Domain.Genres;
using BoardGameApp.Domain.Publishers;
using BoardGameApp.Infrastructure.Games;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Tests;

public class GameRepositoryTests
{
    [Fact]
    public async Task Create_and_get_by_id_persists_game()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);

        var created = await repository.CreateAsync(CreateGame("Azul", 1, 1));

        var found = await repository.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Azul", found.Name);
        Assert.True(found.IsActive);
    }

    [Fact]
    public async Task Update_persists_game_changes()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        var game = await repository.CreateAsync(CreateGame("Before", 1, 1));

        game.Name = "After";
        game.Author = "Updated Author";
        game.MaxPlayers = 5;
        await repository.UpdateAsync(game);

        var updated = await repository.GetByIdAsync(game.Id);

        Assert.NotNull(updated);
        Assert.Equal("After", updated.Name);
        Assert.Equal("Updated Author", updated.Author);
        Assert.Equal(5, updated.MaxPlayers);
    }

    [Fact]
    public async Task Soft_delete_marks_game_inactive_without_removing_it()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        var game = await repository.CreateAsync(CreateGame("Soft Delete", 1, 1));

        await repository.SoftDeleteAsync(game.Id);

        var deleted = await repository.GetByIdAsync(game.Id);

        Assert.NotNull(deleted);
        Assert.False(deleted.IsActive);
    }

    [Fact]
    public async Task List_returns_only_active_games_by_default()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Active Game", 1, 1));
        await repository.CreateAsync(CreateGame("Inactive Game", 1, 1, isActive: false));

        var result = await repository.ListAsync(new GameFilter());

        Assert.Single(result.Items);
        Assert.Equal("Active Game", result.Items[0].Name);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task List_can_include_inactive_games_when_requested()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Active Game", 1, 1));
        await repository.CreateAsync(CreateGame("Inactive Game", 1, 1, isActive: false));

        var result = await repository.ListAsync(new GameFilter(), includeInactive: true);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task List_filters_by_id()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        var target = await repository.CreateAsync(CreateGame("Target Game", 1, 1));
        await repository.CreateAsync(CreateGame("Other Game", 1, 1));

        var result = await repository.ListAsync(new GameFilter(Id: target.Id));

        Assert.Single(result.Items);
        Assert.Equal(target.Id, result.Items[0].Id);
    }

    [Fact]
    public async Task List_filters_by_name()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Azul", 1, 1));
        await repository.CreateAsync(CreateGame("Catan", 1, 1));

        var result = await repository.ListAsync(new GameFilter(Name: "Az"));

        Assert.Single(result.Items);
        Assert.Equal("Azul", result.Items[0].Name);
    }

    [Fact]
    public async Task List_filters_by_author()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Azul", 1, 1, "Michael Kiesling"));
        await repository.CreateAsync(CreateGame("Catan", 1, 1, "Klaus Teuber"));

        var result = await repository.ListAsync(new GameFilter(Author: "Klaus"));

        Assert.Single(result.Items);
        Assert.Equal("Catan", result.Items[0].Name);
    }

    [Fact]
    public async Task List_filters_by_genre()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Azul", 1, 1));
        await repository.CreateAsync(CreateGame("Catan", 1, 2));

        var result = await repository.ListAsync(new GameFilter(GenreId: 2));

        Assert.Single(result.Items);
        Assert.Equal("Catan", result.Items[0].Name);
    }

    [Fact]
    public async Task List_filters_by_publisher()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Azul", 1, 1));
        await repository.CreateAsync(CreateGame("Catan", 2, 1));

        var result = await repository.ListAsync(new GameFilter(PublisherId: 2));

        Assert.Single(result.Items);
        Assert.Equal("Catan", result.Items[0].Name);
    }

    [Fact]
    public async Task List_applies_filters_together()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);
        await repository.CreateAsync(CreateGame("Azul", 1, 1, "Michael Kiesling"));
        await repository.CreateAsync(CreateGame("Azul Summer Pavilion", 2, 1, "Michael Kiesling"));
        await repository.CreateAsync(CreateGame("Catan", 1, 2, "Klaus Teuber"));

        var result = await repository.ListAsync(new GameFilter(
            Name: "Azul",
            Author: "Michael",
            GenreId: 1,
            PublisherId: 1));

        Assert.Single(result.Items);
        Assert.Equal("Azul", result.Items[0].Name);
    }

    [Fact]
    public async Task List_uses_fixed_page_size_of_twenty_and_returns_requested_page()
    {
        await using var dbContext = CreateDbContext();
        await SeedSupportingDataAsync(dbContext);
        var repository = new GameRepository(dbContext);

        for (var index = 1; index <= 25; index++)
        {
            await repository.CreateAsync(CreateGame($"Game {index:00}", 1, 1));
        }

        var firstPage = await repository.ListAsync(new GameFilter());
        var secondPage = await repository.ListAsync(new GameFilter(), page: 2);

        Assert.Equal(20, firstPage.Items.Count);
        Assert.Equal(5, secondPage.Items.Count);
        Assert.Equal(25, firstPage.TotalCount);
        Assert.Equal(20, firstPage.PageSize);
        Assert.Equal(2, secondPage.Page);
        Assert.Equal("Game 21", secondPage.Items[0].Name);
    }

    private static Game CreateGame(
        string name,
        int publisherId,
        int genreId,
        string author = "Author",
        bool isActive = true)
    {
        return new Game
        {
            Name = name,
            PublisherId = publisherId,
            GenreId = genreId,
            Author = author,
            MaxPlayers = 4,
            IsActive = isActive
        };
    }

    private static async Task SeedSupportingDataAsync(AppDbContext dbContext)
    {
        dbContext.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Family" });
        dbContext.Publishers.AddRange(
            new Publisher { Id = 1, Name = "Galapagos" },
            new Publisher { Id = 2, Name = "Devir" });
        await dbContext.SaveChangesAsync();
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
