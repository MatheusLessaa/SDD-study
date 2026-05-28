using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Games;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Domain.Players;

namespace BoardGameApp.Tests;

public class MatchServiceTests
{
    [Fact]
    public async Task Create_parses_player_ids_and_scores_and_calculates_winner()
    {
        var matchRepository = new FakeMatchRepository();
        var service = CreateService(matchRepository, CreateGame(1, maxPlayers: 4));

        var beforeCreate = DateTime.Now;
        var created = await service.CreateAsync(new CreateMatchDto(1, "1, 5, 8", "10, 7, 3", WinnerPlayerId: 999));
        var afterCreate = DateTime.Now;

        Assert.Equal("1,5,8", created.PlayerIds);
        Assert.Equal("10,7,3", created.Scores);
        Assert.Equal(1, created.WinnerPlayerId);
        Assert.InRange(created.CreatedAt, beforeCreate, afterCreate);
    }

    [Fact]
    public async Task Create_defaults_empty_scores_to_zero()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var created = await service.CreateAsync(new CreateMatchDto(1, "1,2,3", string.Empty, WinnerPlayerId: 999));

        Assert.Equal("0,0,0", created.Scores);
        Assert.Equal(2, created.WinnerPlayerId);
    }

    [Fact]
    public async Task Create_defaults_empty_score_items_to_zero()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var created = await service.CreateAsync(new CreateMatchDto(1, "1,2,3", "5,,8", WinnerPlayerId: 999));

        Assert.Equal("5,0,8", created.Scores);
        Assert.Equal(3, created.WinnerPlayerId);
    }

    [Fact]
    public async Task Create_prevents_duplicate_players()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateMatchDto(1, "1,2,1", "5,6,7", WinnerPlayerId: 999)));

        Assert.Contains("duplicate", exception.Message);
    }

    [Fact]
    public async Task Create_validates_max_players_constraint()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 2));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateMatchDto(1, "1,2,3", "5,6,7", WinnerPlayerId: 999)));

        Assert.Contains("max players", exception.Message);
    }

    [Fact]
    public async Task Create_calculates_winner_from_highest_score()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var created = await service.CreateAsync(new CreateMatchDto(1, "10,20,30", "6,12,9", WinnerPlayerId: 999));

        Assert.Equal(20, created.WinnerPlayerId);
    }

    [Fact]
    public async Task Create_ensures_winner_belongs_to_player_ids()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var created = await service.CreateAsync(new CreateMatchDto(1, "10,20,30", "6,12,9", WinnerPlayerId: 999));
        var playerIds = created.PlayerIds.Split(',').Select(int.Parse);

        Assert.Contains(created.WinnerPlayerId, playerIds);
    }

    [Fact]
    public async Task Create_increments_game_times_played_when_match_is_created()
    {
        var game = CreateGame(1, maxPlayers: 4);
        game.TimesPlayed = 2;
        var gameRepository = new FakeGameRepository(game);
        var service = new MatchService(new FakeMatchRepository(), gameRepository, CreateDefaultPlayerRepository());

        await service.CreateAsync(new CreateMatchDto(1, "1,2", "5,8", WinnerPlayerId: 999));

        Assert.Equal(3, game.TimesPlayed);
        Assert.Equal(1, gameRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Create_does_not_increment_game_times_played_when_match_creation_fails_validation()
    {
        var game = CreateGame(1, maxPlayers: 2);
        game.TimesPlayed = 2;
        var gameRepository = new FakeGameRepository(game);
        var service = new MatchService(new FakeMatchRepository(), gameRepository, CreateDefaultPlayerRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateMatchDto(1, "1,2,3", "5,8,1", WinnerPlayerId: 999)));

        Assert.Equal(2, game.TimesPlayed);
        Assert.Equal(0, gameRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Create_throws_when_scores_count_does_not_match_players_count()
    {
        var service = CreateService(new FakeMatchRepository(), CreateGame(1, maxPlayers: 4));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateMatchDto(1, "1,2,3", "5,6", WinnerPlayerId: 999)));

        Assert.Contains("Scores count", exception.Message);
    }

    [Fact]
    public async Task Create_throws_when_game_does_not_exist()
    {
        var service = CreateService(new FakeMatchRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateMatchDto(1, "1,2", "5,6", WinnerPlayerId: 999)));

        Assert.Contains("Game with id", exception.Message);
    }

    [Fact]
    public async Task List_maps_repository_entities_to_view_dtos()
    {
        var matchRepository = new FakeMatchRepository();
        await matchRepository.CreateAsync(new Match
        {
            GameId = 1,
            PlayerIds = "1,2",
            Scores = "2,8",
            WinnerPlayerId = 2,
            CreatedAt = new DateTime(2026, 5, 12, 18, 30, 0)
        });
        var service = CreateService(matchRepository, CreateGame(1, maxPlayers: 2));

        var result = await service.ListAsync(new MatchFilter());

        Assert.Single(result.Items);
        Assert.IsType<MatchViewDto>(result.Items[0]);
        Assert.Equal(2, result.Items[0].WinnerPlayerId);
        Assert.Equal("Azul", result.Items[0].GameName);
        Assert.Equal("Zoe Player, Alice Player", result.Items[0].PlayerNames);
        Assert.Equal("Alice Player", result.Items[0].WinnerPlayerName);
        Assert.Equal(new DateTime(2026, 5, 12, 18, 30, 0), result.Items[0].CreatedAt);
    }

    [Fact]
    public async Task Get_by_id_maps_match_details_with_player_scores()
    {
        var matchRepository = new FakeMatchRepository();
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 1,
            PlayerIds = "1,2,5",
            Scores = "7,11,3",
            WinnerPlayerId = 2,
            CreatedAt = new DateTime(2026, 5, 12, 18, 30, 0)
        });
        var service = CreateService(matchRepository, CreateGame(1, maxPlayers: 3));

        var result = await service.GetByIdAsync(match.Id);

        Assert.NotNull(result);
        Assert.Equal("Azul", result.GameName);
        Assert.Equal("Zoe Player, Alice Player, Bob Player", result.PlayerNames);
        Assert.Equal("Alice Player", result.WinnerPlayerName);
        Assert.Collection(
            result.PlayerScoreDetails,
            playerScore =>
            {
                Assert.Equal(1, playerScore.PlayerId);
                Assert.Equal("Zoe Player", playerScore.PlayerName);
                Assert.Equal(7, playerScore.Score);
            },
            playerScore =>
            {
                Assert.Equal(2, playerScore.PlayerId);
                Assert.Equal("Alice Player", playerScore.PlayerName);
                Assert.Equal(11, playerScore.Score);
            },
            playerScore =>
            {
                Assert.Equal(5, playerScore.PlayerId);
                Assert.Equal("Bob Player", playerScore.PlayerName);
                Assert.Equal(3, playerScore.Score);
            });
    }

    [Fact]
    public async Task Update_scores_changes_only_scores_and_recalculates_winner()
    {
        var matchRepository = new FakeMatchRepository();
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 42,
            PlayerIds = "1,2,3",
            Scores = "2,8,1",
            WinnerPlayerId = 2,
            CreatedAt = new DateTime(2026, 5, 12, 18, 30, 0)
        });
        var service = CreateService(matchRepository, CreateGame(42, maxPlayers: 3));

        var updated = await service.UpdateScoresAsync(new UpdateMatchScoresDto(match.Id, "9,3,7"));

        Assert.Equal(match.Id, updated.Id);
        Assert.Equal(42, updated.GameId);
        Assert.Equal("1,2,3", updated.PlayerIds);
        Assert.Equal("9,3,7", updated.Scores);
        Assert.Equal(1, updated.WinnerPlayerId);
        Assert.Equal(new DateTime(2026, 5, 12, 18, 30, 0), updated.CreatedAt);
    }

    [Fact]
    public async Task Update_scores_keeps_game_and_players_locked()
    {
        var matchRepository = new FakeMatchRepository();
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 99,
            PlayerIds = "3,2,1",
            Scores = "1,2,3",
            WinnerPlayerId = 1
        });
        var service = CreateService(matchRepository, CreateGame(99, maxPlayers: 3));

        var updated = await service.UpdateScoresAsync(new UpdateMatchScoresDto(match.Id, "7,8,9"));

        Assert.Equal(99, updated.GameId);
        Assert.Equal("3,2,1", updated.PlayerIds);
    }

    [Fact]
    public async Task Update_scores_recalculates_winner_with_alphabetical_tie_breaker()
    {
        var matchRepository = new FakeMatchRepository();
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 1,
            PlayerIds = "1,2,3",
            Scores = "1,2,3",
            WinnerPlayerId = 3
        });
        var service = CreateService(matchRepository, CreateGame(1, maxPlayers: 3));

        var updated = await service.UpdateScoresAsync(new UpdateMatchScoresDto(match.Id, "10,10,8"));

        Assert.Equal(2, updated.WinnerPlayerId);
    }

    [Fact]
    public async Task Update_scores_throws_when_score_count_mismatches_players()
    {
        var matchRepository = new FakeMatchRepository();
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 1,
            PlayerIds = "1,2,3",
            Scores = "1,2,3",
            WinnerPlayerId = 3
        });
        var service = CreateService(matchRepository, CreateGame(1, maxPlayers: 3));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateScoresAsync(new UpdateMatchScoresDto(match.Id, "10,8")));

        Assert.Contains("Scores count", exception.Message);
    }

    [Fact]
    public async Task Update_scores_has_no_side_effects_on_game_or_players()
    {
        var matchRepository = new FakeMatchRepository();
        var gameRepository = new FakeGameRepository(CreateGame(1, maxPlayers: 3));
        var playerRepository = new FakePlayerRepository(
            new Player { Id = 1, FullName = "Zoe Player", WhatsApp = "1" },
            new Player { Id = 2, FullName = "Alice Player", WhatsApp = "2" },
            new Player { Id = 3, FullName = "Maria Player", WhatsApp = "3" });
        var match = await matchRepository.CreateAsync(new Match
        {
            GameId = 1,
            PlayerIds = "1,2,3",
            Scores = "1,2,3",
            WinnerPlayerId = 3
        });
        var service = new MatchService(matchRepository, gameRepository, playerRepository);

        await service.UpdateScoresAsync(new UpdateMatchScoresDto(match.Id, "5,9,4"));

        Assert.Equal(0, gameRepository.UpdateCallCount);
        Assert.Equal(0, playerRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Create_breaks_tied_highest_score_by_player_full_name_ascending()
    {
        var playerRepository = new FakePlayerRepository(
            new Player { Id = 1, FullName = "Zoe Player", WhatsApp = "1" },
            new Player { Id = 2, FullName = "Alice Player", WhatsApp = "2" },
            new Player { Id = 3, FullName = "Maria Player", WhatsApp = "3" });
        var service = new MatchService(
            new FakeMatchRepository(),
            new FakeGameRepository(CreateGame(1, maxPlayers: 4)),
            playerRepository);

        var created = await service.CreateAsync(new CreateMatchDto(1, "1,2,3", "10,10,8", WinnerPlayerId: 999));

        Assert.Equal(2, created.WinnerPlayerId);
    }

    private static MatchService CreateService(FakeMatchRepository matchRepository, params Game[] games)
    {
        return new MatchService(
            matchRepository,
            new FakeGameRepository(games),
            CreateDefaultPlayerRepository());
    }

    private static FakePlayerRepository CreateDefaultPlayerRepository()
    {
        return new FakePlayerRepository(
            new Player { Id = 1, FullName = "Zoe Player", WhatsApp = "1" },
            new Player { Id = 2, FullName = "Alice Player", WhatsApp = "2" },
            new Player { Id = 3, FullName = "Maria Player", WhatsApp = "3" },
            new Player { Id = 5, FullName = "Bob Player", WhatsApp = "5" },
            new Player { Id = 8, FullName = "Clara Player", WhatsApp = "8" },
            new Player { Id = 10, FullName = "Diego Player", WhatsApp = "10" },
            new Player { Id = 20, FullName = "Eva Player", WhatsApp = "20" },
            new Player { Id = 30, FullName = "Felipe Player", WhatsApp = "30" });
    }

    private static Game CreateGame(int id, int maxPlayers)
    {
        return new Game
        {
            Id = id,
            Name = "Azul",
            PublisherId = 1,
            GenreId = 1,
            AuthorId = 1,
            MaxPlayers = maxPlayers
        };
    }

    private sealed class FakeMatchRepository : IMatchRepository
    {
        private readonly List<Match> matches = [];
        private int nextId = 1;

        public Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default)
        {
            match.Id = nextId++;
            matches.Add(match);

            return Task.FromResult(match);
        }

        public Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
        {
            var index = matches.FindIndex(storedMatch => storedMatch.Id == match.Id);

            if (index >= 0)
            {
                matches[index] = match;
            }

            return Task.CompletedTask;
        }

        public Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(matches.FirstOrDefault(match => match.Id == id));
        }

        public Task<PagedResult<Match>> ListAsync(
            MatchFilter filter,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var query = matches.AsEnumerable();

            if (filter.Id.HasValue)
            {
                query = query.Where(match => match.Id == filter.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.GameName))
            {
                query = query.Where(match => match.GameId == 1);
            }

            var totalCount = query.Count();
            var items = query
                .OrderByDescending(match => match.Id)
                .Skip((Math.Max(page, 1) - 1) * IMatchRepository.PageSize)
                .Take(IMatchRepository.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Match>(
                items,
                Math.Max(page, 1),
                IMatchRepository.PageSize,
                totalCount));
        }
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly List<Game> games = [];

        public int UpdateCallCount { get; private set; }

        public FakeGameRepository(params Game[] games)
        {
            this.games.AddRange(games);
        }

        public Task<Game> CreateAsync(Game game, CancellationToken cancellationToken = default)
        {
            games.Add(game);

            return Task.FromResult(game);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            UpdateCallCount++;

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
            return Task.FromResult(false);
        }

        public Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<GenreOptionDto>> ListGenreOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<GenreOptionDto>>([]);
        }

        public Task<IReadOnlyList<PublisherOptionDto>> ListPublisherOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<PublisherOptionDto>>([]);
        }

        public Task<PagedResult<Game>> ListAsync(
            GameFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<Game>([], page, IGameRepository.PageSize, 0));
        }
    }

    private sealed class FakePlayerRepository : IPlayerRepository
    {
        private readonly List<Player> players = [];

        public int UpdateCallCount { get; private set; }

        public FakePlayerRepository(params Player[] players)
        {
            this.players.AddRange(players);
        }

        public Task<Player> CreateAsync(Player player, CancellationToken cancellationToken = default)
        {
            players.Add(player);

            return Task.FromResult(player);
        }

        public Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
        {
            UpdateCallCount++;

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
            return Task.FromResult(false);
        }

        public Task<bool> ExistsByWhatsAppAsync(
            string whatsApp,
            int? excludingId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<PagedResult<Player>> ListAsync(
            PlayerFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<Player>([], page, IPlayerRepository.PageSize, 0));
        }
    }
}
