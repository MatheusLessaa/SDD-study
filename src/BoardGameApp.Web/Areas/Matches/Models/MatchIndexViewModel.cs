using BoardGameApp.Application.Common;
using BoardGameApp.Application.Matches;

namespace BoardGameApp.Web.Areas.Matches.Models;

public sealed class MatchIndexViewModel
{
    public required PagedResult<MatchViewDto> Matches { get; init; }

    public int? Id { get; init; }

    public string? GameName { get; init; }

    public int PreviousPage => Math.Max(Matches.Page - 1, 1);

    public int NextPage => Matches.Page + 1;

    public bool HasPreviousPage => Matches.Page > 1;

    public bool HasNextPage => Matches.Page * Matches.PageSize < Matches.TotalCount;

    public int FirstItemNumber => Matches.TotalCount == 0
        ? 0
        : ((Matches.Page - 1) * Matches.PageSize) + 1;

    public int LastItemNumber => Math.Min(Matches.Page * Matches.PageSize, Matches.TotalCount);
}
