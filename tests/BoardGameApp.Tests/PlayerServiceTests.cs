using BoardGameApp.Application.Common;
using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Players;

namespace BoardGameApp.Tests;

public class PlayerServiceTests
{
    [Fact]
    public async Task Create_enforces_unique_full_name()
    {
        var repository = new FakePlayerRepository();
        await repository.CreateAsync(new Player { FullName = "Ada Lovelace", WhatsApp = "1111" });
        var service = new PlayerService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreatePlayerDto("Ada Lovelace", "2222")));

        Assert.Contains("full name", exception.Message);
    }

    [Fact]
    public async Task Create_enforces_unique_whatsapp()
    {
        var repository = new FakePlayerRepository();
        await repository.CreateAsync(new Player { FullName = "Ada Lovelace", WhatsApp = "1111" });
        var service = new PlayerService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreatePlayerDto("Grace Hopper", "1111")));

        Assert.Contains("WhatsApp", exception.Message);
    }

    [Fact]
    public async Task Create_returns_view_dto_when_player_is_valid()
    {
        var repository = new FakePlayerRepository();
        var service = new PlayerService(repository);

        var created = await service.CreateAsync(new CreatePlayerDto("Grace Hopper", "3333"));

        Assert.Equal(1, created.Id);
        Assert.Equal("Grace Hopper", created.FullName);
        Assert.Equal("3333", created.WhatsApp);
        Assert.True(created.IsActive);
    }

    [Fact]
    public async Task Update_allows_same_player_to_keep_full_name_and_whatsapp()
    {
        var repository = new FakePlayerRepository();
        var player = await repository.CreateAsync(new Player { FullName = "Before", WhatsApp = "4444" });
        var service = new PlayerService(repository);

        var updated = await service.UpdateAsync(new UpdatePlayerDto(player.Id, "Before", "4444", true));

        Assert.Equal(player.Id, updated.Id);
        Assert.Equal("Before", updated.FullName);
        Assert.Equal("4444", updated.WhatsApp);
    }

    [Fact]
    public async Task Update_enforces_unique_full_name_against_other_players()
    {
        var repository = new FakePlayerRepository();
        await repository.CreateAsync(new Player { FullName = "Existing", WhatsApp = "5555" });
        var target = await repository.CreateAsync(new Player { FullName = "Target", WhatsApp = "6666" });
        var service = new PlayerService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAsync(new UpdatePlayerDto(target.Id, "Existing", "6666", true)));

        Assert.Contains("full name", exception.Message);
    }

    [Fact]
    public async Task Deactivate_marks_player_inactive_without_hard_delete()
    {
        var repository = new FakePlayerRepository();
        var player = await repository.CreateAsync(new Player { FullName = "Target", WhatsApp = "7777" });
        var service = new PlayerService(repository);

        await service.DeactivateAsync(player.Id);

        var stored = await repository.GetByIdAsync(player.Id);
        Assert.NotNull(stored);
        Assert.False(stored.IsActive);
    }

    [Fact]
    public async Task Get_by_id_returns_null_when_player_does_not_exist()
    {
        var service = new PlayerService(new FakePlayerRepository());

        var player = await service.GetByIdAsync(999);

        Assert.Null(player);
    }

    [Fact]
    public async Task List_maps_repository_entities_to_view_dtos()
    {
        var repository = new FakePlayerRepository();
        await repository.CreateAsync(new Player { FullName = "Ada Lovelace", WhatsApp = "1111" });
        var service = new PlayerService(repository);

        var result = await service.ListAsync(new PlayerFilter());

        Assert.Single(result.Items);
        Assert.IsType<PlayerViewDto>(result.Items[0]);
        Assert.Equal("Ada Lovelace", result.Items[0].FullName);
    }

    private sealed class FakePlayerRepository : IPlayerRepository
    {
        private readonly List<Player> players = [];
        private int nextId = 1;

        public Task<Player> CreateAsync(Player player, CancellationToken cancellationToken = default)
        {
            player.Id = nextId++;
            players.Add(player);

            return Task.FromResult(player);
        }

        public Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
        {
            var index = players.FindIndex(storedPlayer => storedPlayer.Id == player.Id);

            if (index >= 0)
            {
                players[index] = player;
            }

            return Task.CompletedTask;
        }

        public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(players.FirstOrDefault(player => player.Id == id));
        }

        public Task<bool> ExistsByFullNameAsync(
            string fullName,
            int? excludingId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(players.Any(player =>
                player.FullName == fullName
                && (!excludingId.HasValue || player.Id != excludingId.Value)));
        }

        public Task<bool> ExistsByWhatsAppAsync(
            string whatsApp,
            int? excludingId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(players.Any(player =>
                player.WhatsApp == whatsApp
                && (!excludingId.HasValue || player.Id != excludingId.Value)));
        }

        public Task<PagedResult<Player>> ListAsync(
            PlayerFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            var query = players.AsEnumerable();

            if (!includeInactive)
            {
                query = query.Where(player => player.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(player => player.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrWhiteSpace(filter.WhatsApp))
            {
                query = query.Where(player => player.WhatsApp.Contains(filter.WhatsApp));
            }

            var items = query
                .OrderBy(player => player.FullName)
                .Skip((Math.Max(page, 1) - 1) * IPlayerRepository.PageSize)
                .Take(IPlayerRepository.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Player>(
                items,
                Math.Max(page, 1),
                IPlayerRepository.PageSize,
                query.Count()));
        }
    }
}
