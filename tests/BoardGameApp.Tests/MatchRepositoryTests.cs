using BoardGameApp.Application.Matches;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Infrastructure.Matches;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Tests;

public class MatchRepositoryTests
{
    [Fact]
    public async Task Create_and_get_by_id_persists_match()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);

        var created = await repository.CreateAsync(CreateMatch(gameId: 3));

        var found = await repository.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal(3, found.GameId);
        Assert.Equal("1,5,8", found.PlayerIds);
        Assert.Equal("10,7,3", found.Scores);
        Assert.Equal(1, found.WinnerPlayerId);
    }

    [Fact]
    public async Task Update_persists_match_changes()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);
        var match = await repository.CreateAsync(CreateMatch(gameId: 4));

        match.Scores = "2,9,4";
        match.WinnerPlayerId = 5;
        await repository.UpdateAsync(match);

        var updated = await repository.GetByIdAsync(match.Id);

        Assert.NotNull(updated);
        Assert.Equal("2,9,4", updated.Scores);
        Assert.Equal(5, updated.WinnerPlayerId);
    }

    [Fact]
    public async Task List_returns_matches_ordered_by_id_descending()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);
        var first = await repository.CreateAsync(CreateMatch(gameId: 1));
        var second = await repository.CreateAsync(CreateMatch(gameId: 1));

        var result = await repository.ListAsync(new MatchFilter());

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(second.Id, result.Items[0].Id);
        Assert.Equal(first.Id, result.Items[1].Id);
    }

    [Fact]
    public async Task List_filters_by_id()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);
        var target = await repository.CreateAsync(CreateMatch(gameId: 1));
        await repository.CreateAsync(CreateMatch(gameId: 2));

        var result = await repository.ListAsync(new MatchFilter(Id: target.Id));

        Assert.Single(result.Items);
        Assert.Equal(target.Id, result.Items[0].Id);
    }

    [Fact]
    public async Task List_filters_by_game_id()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);
        await repository.CreateAsync(CreateMatch(gameId: 1));
        await repository.CreateAsync(CreateMatch(gameId: 2));
        await repository.CreateAsync(CreateMatch(gameId: 2));

        var result = await repository.ListAsync(new MatchFilter(GameId: 2));

        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, match => Assert.Equal(2, match.GameId));
    }

    [Fact]
    public async Task List_applies_id_and_game_id_filters_together()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);
        var target = await repository.CreateAsync(CreateMatch(gameId: 7));
        await repository.CreateAsync(CreateMatch(gameId: 8));

        var result = await repository.ListAsync(new MatchFilter(target.Id, 7));

        Assert.Single(result.Items);
        Assert.Equal(target.Id, result.Items[0].Id);
        Assert.Equal(7, result.Items[0].GameId);
    }

    [Fact]
    public async Task List_uses_fixed_page_size_of_twenty()
    {
        await using var dbContext = CreateDbContext();
        var repository = new MatchRepository(dbContext);

        for (var index = 1; index <= 25; index++)
        {
            await repository.CreateAsync(CreateMatch(gameId: 1));
        }

        var firstPage = await repository.ListAsync(new MatchFilter());
        var secondPage = await repository.ListAsync(new MatchFilter(), page: 2);

        Assert.Equal(20, firstPage.Items.Count);
        Assert.Equal(5, secondPage.Items.Count);
        Assert.Equal(25, firstPage.TotalCount);
        Assert.Equal(20, firstPage.PageSize);
    }

    private static Match CreateMatch(int gameId)
    {
        return new Match
        {
            GameId = gameId,
            PlayerIds = "1,5,8",
            Scores = "10,7,3",
            WinnerPlayerId = 1
        };
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
