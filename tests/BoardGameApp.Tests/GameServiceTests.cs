using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Domain.Games;

namespace BoardGameApp.Tests;

public class GameServiceTests
{
    [Fact]
    public async Task Create_enforces_unique_name_and_publisher()
    {
        var repository = new FakeGameRepository();
        await repository.CreateAsync(CreateGame("Azul", publisherId: 1));
        var service = new GameService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateGameDto("Azul", 1, 1, 1, 4)));

        Assert.Contains("already in use", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Create_validates_max_players(int maxPlayers)
    {
        var service = new GameService(new FakeGameRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateGameDto("Azul", 1, 1, 1, maxPlayers)));

        Assert.Contains("MaxPlayers", exception.Message);
    }

    [Fact]
    public async Task Create_returns_view_dto_when_game_is_valid()
    {
        var service = new GameService(new FakeGameRepository());

        var created = await service.CreateAsync(new CreateGameDto("Azul", 1, 2, 1, 4));

        Assert.Equal(1, created.Id);
        Assert.Equal("Azul", created.Name);
        Assert.Equal(1, created.PublisherId);
        Assert.Equal(2, created.GenreId);
        Assert.Equal("Galapagos", created.PublisherName);
        Assert.Equal("Family", created.GenreName);
        Assert.Equal(1, created.AuthorId);
        Assert.Equal("Michael Kiesling", created.AuthorName);
        Assert.Equal(4, created.MaxPlayers);
        Assert.True(created.IsActive);
    }

    [Fact]
    public async Task Create_rejects_invalid_author_id()
    {
        var service = new GameService(new FakeGameRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateGameDto("Azul", 1, 1, 99, 4)));

        Assert.Contains("AuthorId", exception.Message);
    }

    [Fact]
    public async Task Update_allows_same_game_to_keep_name_and_publisher()
    {
        var repository = new FakeGameRepository();
        var game = await repository.CreateAsync(CreateGame("Azul", publisherId: 1));
        var service = new GameService(repository);

        var updated = await service.UpdateAsync(new UpdateGameDto(
            game.Id,
            "Azul",
            1,
            2,
            2,
            3,
            4,
            true));

        Assert.Equal(game.Id, updated.Id);
        Assert.Equal("Azul", updated.Name);
        Assert.Equal(3, updated.TimesPlayed);
        Assert.Equal(2, updated.AuthorId);
        Assert.Equal("Klaus Teuber", updated.AuthorName);
    }

    [Fact]
    public async Task Update_enforces_unique_name_and_publisher_against_other_games()
    {
        var repository = new FakeGameRepository();
        await repository.CreateAsync(CreateGame("Existing", publisherId: 1));
        var target = await repository.CreateAsync(CreateGame("Target", publisherId: 1));
        var service = new GameService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAsync(new UpdateGameDto(target.Id, "Existing", 1, 1, 1, 0, 4, true)));

        Assert.Contains("already in use", exception.Message);
    }

    [Fact]
    public async Task Deactivate_marks_game_inactive_without_hard_delete()
    {
        var repository = new FakeGameRepository();
        var game = await repository.CreateAsync(CreateGame("Target", publisherId: 1));
        var service = new GameService(repository);

        await service.DeactivateAsync(game.Id);

        var stored = await repository.GetByIdAsync(game.Id);
        Assert.NotNull(stored);
        Assert.False(stored.IsActive);
    }

    [Fact]
    public async Task Activate_marks_game_active()
    {
        var repository = new FakeGameRepository();
        var game = await repository.CreateAsync(CreateGame("Target", publisherId: 1, isActive: false));
        var service = new GameService(repository);

        await service.ActivateAsync(game.Id);

        var stored = await repository.GetByIdAsync(game.Id);
        Assert.NotNull(stored);
        Assert.True(stored.IsActive);
    }

    [Fact]
    public async Task Get_by_id_returns_null_when_game_does_not_exist()
    {
        var service = new GameService(new FakeGameRepository());

        var game = await service.GetByIdAsync(999);

        Assert.Null(game);
    }

    [Fact]
    public async Task List_maps_repository_entities_to_view_dtos()
    {
        var repository = new FakeGameRepository();
        await repository.CreateAsync(CreateGame("Azul", publisherId: 1));
        var service = new GameService(repository);

        var result = await service.ListAsync(new GameFilter());

        Assert.Single(result.Items);
        Assert.IsType<GameViewDto>(result.Items[0]);
        Assert.Equal("Azul", result.Items[0].Name);
        Assert.Equal("Galapagos", result.Items[0].PublisherName);
        Assert.Equal("Strategy", result.Items[0].GenreName);
    }

    [Fact]
    public async Task List_genre_options_returns_genres_for_filter_dropdown()
    {
        var service = new GameService(new FakeGameRepository());

        var result = await service.ListGenreOptionsAsync();

        Assert.Collection(
            result,
            genre =>
            {
                Assert.Equal(1, genre.Id);
                Assert.Equal("Strategy", genre.Name);
            },
            genre =>
            {
                Assert.Equal(2, genre.Id);
                Assert.Equal("Family", genre.Name);
            },
            genre =>
            {
                Assert.Equal(3, genre.Id);
                Assert.Equal("Party", genre.Name);
            });
    }

    private static Game CreateGame(string name, int publisherId, bool isActive = true)
    {
        return new Game
        {
            Name = name,
            PublisherId = publisherId,
            GenreId = 1,
            AuthorId = 1,
            MaxPlayers = 4,
            IsActive = isActive
        };
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly List<Game> games = [];
        private int nextId = 1;

        public Task<Game> CreateAsync(Game game, CancellationToken cancellationToken = default)
        {
            game.Id = nextId++;
            games.Add(game);

            return Task.FromResult(game);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            var index = games.FindIndex(storedGame => storedGame.Id == game.Id);

            if (index >= 0)
            {
                games[index] = game;
            }

            return Task.CompletedTask;
        }

        public Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(games.FirstOrDefault(game => game.Id == id));
        }

        public Task<bool> ExistsByNameAndPublisherAsync(
            string name,
            int publisherId,
            int? excludingId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(games.Any(game =>
                game.Name == name
                && game.PublisherId == publisherId
                && (!excludingId.HasValue || game.Id != excludingId.Value)));
        }

        public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var game = await GetByIdAsync(id, cancellationToken);

            if (game is not null)
            {
                game.IsActive = false;
            }
        }

        public Task<IReadOnlyList<GenreOptionDto>> ListGenreOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<GenreOptionDto> options =
            [
                new GenreOptionDto(1, "Strategy"),
                new GenreOptionDto(2, "Family"),
                new GenreOptionDto(3, "Party")
            ];

            return Task.FromResult(options);
        }

        public Task<PagedResult<Game>> ListAsync(
            GameFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            var query = games.AsEnumerable();

            if (!includeInactive)
            {
                query = query.Where(game => game.IsActive);
            }

            if (filter.Id.HasValue)
            {
                query = query.Where(game => game.Id == filter.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(game => game.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Author))
            {
                query = query.Where(game => ResolveAuthorName(game.AuthorId).Contains(filter.Author));
            }

            if (filter.GenreId.HasValue)
            {
                query = query.Where(game => game.GenreId == filter.GenreId.Value);
            }

            if (filter.PublisherId.HasValue)
            {
                query = query.Where(game => game.PublisherId == filter.PublisherId.Value);
            }

            var totalCount = query.Count();
            var items = query
                .OrderBy(game => game.Name)
                .Skip((Math.Max(page, 1) - 1) * IGameRepository.PageSize)
                .Take(IGameRepository.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Game>(
                items,
                Math.Max(page, 1),
                IGameRepository.PageSize,
                totalCount));
        }

        private static string ResolveAuthorName(int authorId)
        {
            return authorId switch
            {
                1 => "Michael Kiesling",
                2 => "Klaus Teuber",
                3 => "Jacob Fryxelius",
                _ => $"#{authorId}"
            };
        }
    }
}
