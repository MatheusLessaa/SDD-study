using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public sealed class MatchService : IMatchService
{
    private readonly IMatchRepository matchRepository;
    private readonly IGameRepository gameRepository;
    private readonly IPlayerRepository playerRepository;

    public MatchService(
        IMatchRepository matchRepository,
        IGameRepository gameRepository,
        IPlayerRepository playerRepository)
    {
        this.matchRepository = matchRepository;
        this.gameRepository = gameRepository;
        this.playerRepository = playerRepository;
    }

    public async Task<MatchViewDto> CreateAsync(
        CreateMatchDto dto,
        CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(dto.GameId, cancellationToken)
            ?? throw new InvalidOperationException($"Game with id '{dto.GameId}' was not found.");
        var playerIds = ParsePlayerIds(dto.PlayerIds);
        var scores = ParseScores(dto.Scores, playerIds.Count);

        if (playerIds.Count > game.MaxPlayers)
        {
            throw new InvalidOperationException($"Match has '{playerIds.Count}' players, but game max players is '{game.MaxPlayers}'.");
        }

        var winnerPlayerId = await CalculateWinnerPlayerIdAsync(playerIds, scores, cancellationToken);
        var match = new Match
        {
            GameId = dto.GameId,
            PlayerIds = string.Join(",", playerIds),
            Scores = string.Join(",", scores),
            WinnerPlayerId = winnerPlayerId,
            CreatedAt = DateTime.Now
        };

        var created = await matchRepository.CreateAsync(match, cancellationToken);
        game.TimesPlayed++;
        await gameRepository.UpdateAsync(game, cancellationToken);

        return MatchViewDto.FromEntity(created);
    }

    public async Task<MatchViewDto> UpdateScoresAsync(
        UpdateMatchScoresDto dto,
        CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Match with id '{dto.Id}' was not found.");
        var playerIds = ParsePlayerIds(match.PlayerIds);
        var scores = ParseScores(dto.Scores, playerIds.Count);

        match.Scores = string.Join(",", scores);
        match.WinnerPlayerId = await CalculateWinnerPlayerIdAsync(playerIds, scores, cancellationToken);

        await matchRepository.UpdateAsync(match, cancellationToken);

        return MatchViewDto.FromEntity(match);
    }

    public async Task<MatchViewDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(id, cancellationToken);

        return match is null ? null : await EnrichForDisplayAsync(match, cancellationToken);
    }

    public async Task<PagedResult<MatchViewDto>> ListAsync(
        MatchFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var matches = await matchRepository.ListAsync(filter, page, cancellationToken);
        var items = new List<MatchViewDto>();

        foreach (var match in matches.Items)
        {
            items.Add(await EnrichForDisplayAsync(match, cancellationToken));
        }

        return new PagedResult<MatchViewDto>(
            items,
            matches.Page,
            matches.PageSize,
            matches.TotalCount);
    }

    private async Task<MatchViewDto> EnrichForDisplayAsync(
        Match match,
        CancellationToken cancellationToken)
    {
        var game = await gameRepository.GetByIdAsync(match.GameId, cancellationToken);
        var playerIds = ParsePlayerIds(match.PlayerIds);
        var scores = ParseScores(match.Scores, playerIds.Count);
        var playerNames = new List<string>();
        var playerScores = new List<MatchPlayerScoreDto>();
        var winnerPlayerName = string.Empty;

        for (var index = 0; index < playerIds.Count; index++)
        {
            var playerId = playerIds[index];
            var player = await playerRepository.GetByIdAsync(playerId, cancellationToken);
            var playerName = player?.FullName ?? $"Player #{playerId}";

            playerNames.Add(playerName);
            playerScores.Add(new MatchPlayerScoreDto(playerId, playerName, scores[index]));

            if (playerId == match.WinnerPlayerId)
            {
                winnerPlayerName = playerName;
            }
        }

        if (string.IsNullOrWhiteSpace(winnerPlayerName))
        {
            var winner = await playerRepository.GetByIdAsync(match.WinnerPlayerId, cancellationToken);
            winnerPlayerName = winner?.FullName ?? $"Player #{match.WinnerPlayerId}";
        }

        return MatchViewDto.FromEntity(match) with
        {
            GameName = game?.Name ?? $"Game #{match.GameId}",
            PlayerNames = string.Join(", ", playerNames),
            WinnerPlayerName = winnerPlayerName,
            PlayerScores = playerScores
        };
    }

    private static IReadOnlyList<int> ParsePlayerIds(string playerIds)
    {
        var parsedPlayerIds = playerIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParsePositiveInt)
            .ToList();

        if (parsedPlayerIds.Count == 0)
        {
            throw new InvalidOperationException("A match must contain at least one player.");
        }

        if (parsedPlayerIds.Count != parsedPlayerIds.Distinct().Count())
        {
            throw new InvalidOperationException("A match cannot contain duplicate players.");
        }

        return parsedPlayerIds;
    }

    private static IReadOnlyList<int> ParseScores(string scores, int playerCount)
    {
        if (string.IsNullOrWhiteSpace(scores))
        {
            return Enumerable.Repeat(0, playerCount).ToList();
        }

        var scoreParts = scores.Split(',', StringSplitOptions.TrimEntries);

        if (scoreParts.Length != playerCount)
        {
            throw new InvalidOperationException("Scores count must match PlayerIds count.");
        }

        return scoreParts
            .Select(score => string.IsNullOrWhiteSpace(score) ? 0 : ParseNonNegativeInt(score))
            .ToList();
    }

    private async Task<int> CalculateWinnerPlayerIdAsync(
        IReadOnlyList<int> playerIds,
        IReadOnlyList<int> scores,
        CancellationToken cancellationToken)
    {
        var highestScore = scores.Max();
        var tiedPlayerIds = playerIds
            .Where((_, index) => scores[index] == highestScore)
            .ToList();

        if (tiedPlayerIds.Count == 1)
        {
            return tiedPlayerIds[0];
        }

        var tiedPlayers = new List<(int Id, string FullName)>();

        foreach (var playerId in tiedPlayerIds)
        {
            var player = await playerRepository.GetByIdAsync(playerId, cancellationToken)
                ?? throw new InvalidOperationException($"Player with id '{playerId}' was not found.");

            tiedPlayers.Add((player.Id, player.FullName));
        }

        return tiedPlayers
            .OrderBy(player => player.FullName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(player => player.Id)
            .First()
            .Id;
    }

    private static int ParsePositiveInt(string value)
    {
        if (!int.TryParse(value, out var parsedValue) || parsedValue < 1)
        {
            throw new InvalidOperationException($"Player id '{value}' is invalid.");
        }

        return parsedValue;
    }

    private static int ParseNonNegativeInt(string value)
    {
        if (!int.TryParse(value, out var parsedValue) || parsedValue < 0)
        {
            throw new InvalidOperationException($"Score '{value}' is invalid.");
        }

        return parsedValue;
    }
}
