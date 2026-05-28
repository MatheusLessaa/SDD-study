using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;

namespace BoardGameApp.Web.Areas.Games.Models;

public sealed class GameIndexViewModel
{
    public required PagedResult<GameViewDto> Games { get; init; }

    public required IReadOnlyList<GenreOptionDto> GenreOptions { get; init; }

    public required IReadOnlyList<PublisherOptionDto> PublisherOptions { get; init; }

    public int? Id { get; init; }

    public string? Name { get; init; }

    public string? Author { get; init; }

    public int? GenreId { get; init; }

    public int? PublisherId { get; init; }

    public bool IncludeInactive { get; init; }

    public int PreviousPage => Math.Max(Games.Page - 1, 1);

    public int NextPage => Games.Page + 1;

    public bool HasPreviousPage => Games.Page > 1;

    public bool HasNextPage => Games.Page * Games.PageSize < Games.TotalCount;

    public int FirstItemNumber => Games.TotalCount == 0
        ? 0
        : ((Games.Page - 1) * Games.PageSize) + 1;

    public int LastItemNumber => Math.Min(Games.Page * Games.PageSize, Games.TotalCount);
}
