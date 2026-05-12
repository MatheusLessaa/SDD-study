using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Games;

public interface IGameService
{
    Task<GameViewDto> CreateAsync(CreateGameDto dto, CancellationToken cancellationToken = default);

    Task<GameViewDto> UpdateAsync(UpdateGameDto dto, CancellationToken cancellationToken = default);

    Task ActivateAsync(int id, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);

    Task<GameViewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<GameViewDto>> ListAsync(
        GameFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);
}
