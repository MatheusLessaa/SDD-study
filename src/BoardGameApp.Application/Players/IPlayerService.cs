using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Players;

public interface IPlayerService
{
    Task<PlayerViewDto> CreateAsync(CreatePlayerDto dto, CancellationToken cancellationToken = default);

    Task<PlayerViewDto> UpdateAsync(UpdatePlayerDto dto, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);

    Task<PlayerViewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<PlayerViewDto>> ListAsync(
        PlayerFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);
}
