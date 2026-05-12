using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public interface IMatchRepository
{
    const int PageSize = 20;

    Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default);

    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);

    Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<Match>> ListAsync(
        MatchFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default);
}
