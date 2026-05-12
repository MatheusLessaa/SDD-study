using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Players;

namespace BoardGameApp.Application.Players;

public interface IPlayerRepository
{
    const int PageSize = 20;

    Task<Player> CreateAsync(Player player, CancellationToken cancellationToken = default);

    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);

    Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFullNameAsync(
        string fullName,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByWhatsAppAsync(
        string whatsApp,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Player>> ListAsync(
        PlayerFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);
}
