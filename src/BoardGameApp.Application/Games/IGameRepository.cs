using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Games;

namespace BoardGameApp.Application.Games;

public interface IGameRepository
{
    const int PageSize = 20;

    Task<Game> CreateAsync(Game game, CancellationToken cancellationToken = default);

    Task UpdateAsync(Game game, CancellationToken cancellationToken = default);

    Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAndPublisherAsync(
        string name,
        int publisherId,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GenreOptionDto>> ListGenreOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PublisherOptionDto>> ListPublisherOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<Game>> ListAsync(
        GameFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);
}
