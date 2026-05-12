using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Matches;

public interface IMatchService
{
    Task<MatchViewDto> CreateAsync(CreateMatchDto dto, CancellationToken cancellationToken = default);

    Task<MatchViewDto> UpdateScoresAsync(
        UpdateMatchScoresDto dto,
        CancellationToken cancellationToken = default);

    Task<MatchViewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<MatchViewDto>> ListAsync(
        MatchFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default);
}
