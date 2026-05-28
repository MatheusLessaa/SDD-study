using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Games;

public sealed class GameService : IGameService
{
    private readonly IGameRepository gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<GameViewDto> CreateAsync(
        CreateGameDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateAuthorId(dto.AuthorId);
        ValidateMaxPlayers(dto.MaxPlayers);
        await EnsureUniqueNameAndPublisherAsync(dto.Name, dto.PublisherId, excludingId: null, cancellationToken);

        var game = await gameRepository.CreateAsync(dto.ToEntity(), cancellationToken);

        return GameViewDto.FromEntity(game);
    }

    public async Task<GameViewDto> UpdateAsync(
        UpdateGameDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateAuthorId(dto.AuthorId);
        ValidateMaxPlayers(dto.MaxPlayers);

        var game = await gameRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Game with id '{dto.Id}' was not found.");

        await EnsureUniqueNameAndPublisherAsync(dto.Name, dto.PublisherId, dto.Id, cancellationToken);

        dto.ApplyTo(game);
        await gameRepository.UpdateAsync(game, cancellationToken);

        return GameViewDto.FromEntity(game);
    }

    public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Game with id '{id}' was not found.");

        game.IsActive = true;
        await gameRepository.UpdateAsync(game, cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Game with id '{id}' was not found.");

        game.IsActive = false;
        await gameRepository.UpdateAsync(game, cancellationToken);
    }

    public async Task<GameViewDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var game = await gameRepository.GetByIdAsync(id, cancellationToken);

        return game is null ? null : GameViewDto.FromEntity(game);
    }

    public Task<IReadOnlyList<GenreOptionDto>> ListGenreOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return gameRepository.ListGenreOptionsAsync(cancellationToken);
    }

    public Task<IReadOnlyList<PublisherOptionDto>> ListPublisherOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return gameRepository.ListPublisherOptionsAsync(cancellationToken);
    }

    public async Task<PagedResult<GameViewDto>> ListAsync(
        GameFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var games = await gameRepository.ListAsync(
            filter,
            page,
            includeInactive,
            cancellationToken);
        var items = games.Items
            .Select(GameViewDto.FromEntity)
            .ToList();

        return new PagedResult<GameViewDto>(
            items,
            games.Page,
            games.PageSize,
            games.TotalCount);
    }

    private static void ValidateMaxPlayers(int maxPlayers)
    {
        if (maxPlayers < 1)
        {
            throw new InvalidOperationException("MaxPlayers must be greater than zero.");
        }
    }

    private static void ValidateAuthorId(int authorId)
    {
        if (authorId is < 1 or > 3)
        {
            throw new InvalidOperationException("AuthorId must reference an existing author.");
        }
    }

    private async Task EnsureUniqueNameAndPublisherAsync(
        string name,
        int publisherId,
        int? excludingId,
        CancellationToken cancellationToken)
    {
        if (await gameRepository.ExistsByNameAndPublisherAsync(name, publisherId, excludingId, cancellationToken))
        {
            throw new InvalidOperationException($"Game name '{name}' is already in use for publisher '{publisherId}'.");
        }
    }
}
