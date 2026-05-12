using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Players;
using BoardGameApp.Infrastructure.Persistence;
using BoardGameApp.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Tests;

public class PlayerRepositoryTests
{
    [Fact]
    public async Task Create_and_get_by_id_persists_player()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);

        var created = await repository.CreateAsync(new Player
        {
            FullName = "Ada Lovelace",
            WhatsApp = "1111"
        });

        var found = await repository.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Ada Lovelace", found.FullName);
        Assert.Equal("1111", found.WhatsApp);
        Assert.True(found.IsActive);
    }

    [Fact]
    public async Task Update_persists_player_changes()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);
        var player = await repository.CreateAsync(new Player
        {
            FullName = "Before",
            WhatsApp = "2222"
        });

        player.FullName = "After";
        player.WhatsApp = "3333";
        player.IsActive = false;
        await repository.UpdateAsync(player);

        var updated = await repository.GetByIdAsync(player.Id);

        Assert.NotNull(updated);
        Assert.Equal("After", updated.FullName);
        Assert.Equal("3333", updated.WhatsApp);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task List_returns_only_active_players_by_default()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);
        await repository.CreateAsync(new Player { FullName = "Active Player", WhatsApp = "4444" });
        await repository.CreateAsync(new Player { FullName = "Inactive Player", WhatsApp = "5555", IsActive = false });

        var result = await repository.ListAsync(new PlayerFilter());

        Assert.Single(result.Items);
        Assert.Equal("Active Player", result.Items[0].FullName);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task List_can_include_inactive_players_when_requested()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);
        await repository.CreateAsync(new Player { FullName = "Active Player", WhatsApp = "4444" });
        await repository.CreateAsync(new Player { FullName = "Inactive Player", WhatsApp = "5555", IsActive = false });

        var result = await repository.ListAsync(new PlayerFilter(), includeInactive: true);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task List_applies_full_name_and_whatsapp_filters_together()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);
        await repository.CreateAsync(new Player { FullName = "Alice Cooper", WhatsApp = "119999" });
        await repository.CreateAsync(new Player { FullName = "Alice Smith", WhatsApp = "218888" });
        await repository.CreateAsync(new Player { FullName = "Bob Cooper", WhatsApp = "119999" });

        var result = await repository.ListAsync(new PlayerFilter("Alice", "119"));

        Assert.Single(result.Items);
        Assert.Equal("Alice Cooper", result.Items[0].FullName);
    }

    [Fact]
    public async Task List_uses_fixed_page_size_of_twenty()
    {
        await using var dbContext = CreateDbContext();
        var repository = new PlayerRepository(dbContext);

        for (var index = 1; index <= 25; index++)
        {
            await repository.CreateAsync(new Player
            {
                FullName = $"Player {index:00}",
                WhatsApp = $"9000{index:00}"
            });
        }

        var firstPage = await repository.ListAsync(new PlayerFilter());
        var secondPage = await repository.ListAsync(new PlayerFilter(), page: 2);

        Assert.Equal(20, firstPage.Items.Count);
        Assert.Equal(5, secondPage.Items.Count);
        Assert.Equal(25, firstPage.TotalCount);
        Assert.Equal(20, firstPage.PageSize);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
