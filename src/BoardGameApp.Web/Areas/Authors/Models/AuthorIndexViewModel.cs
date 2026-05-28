using BoardGameApp.Application.Authors;
using BoardGameApp.Application.Common;

namespace BoardGameApp.Web.Areas.Authors.Models;

public sealed class AuthorIndexViewModel
{
    public required PagedResult<AuthorViewDto> Authors { get; init; }

    public string? Name { get; init; }

    public string? StatusMessage { get; init; }

    public int PreviousPage => Math.Max(Authors.Page - 1, 1);

    public int NextPage => Authors.Page + 1;

    public bool HasPreviousPage => Authors.Page > 1;

    public bool HasNextPage => Authors.Page * Authors.PageSize < Authors.TotalCount;

    public int FirstItemNumber => Authors.TotalCount == 0
        ? 0
        : ((Authors.Page - 1) * Authors.PageSize) + 1;

    public int LastItemNumber => Math.Min(Authors.Page * Authors.PageSize, Authors.TotalCount);
}
