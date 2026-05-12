using BoardGameApp.Application.Common;
using BoardGameApp.Application.Players;

namespace BoardGameApp.Web.Areas.Players.Models;

public sealed class PlayerIndexViewModel
{
    public required PagedResult<PlayerViewDto> Players { get; init; }

    public string? FullName { get; init; }

    public string? WhatsApp { get; init; }

    public bool IncludeInactive { get; init; }

    public int PreviousPage => Math.Max(Players.Page - 1, 1);

    public int NextPage => Players.Page + 1;

    public bool HasPreviousPage => Players.Page > 1;

    public bool HasNextPage => Players.Page * Players.PageSize < Players.TotalCount;

    public int FirstItemNumber => Players.TotalCount == 0
        ? 0
        : ((Players.Page - 1) * Players.PageSize) + 1;

    public int LastItemNumber => Math.Min(Players.Page * Players.PageSize, Players.TotalCount);
}
