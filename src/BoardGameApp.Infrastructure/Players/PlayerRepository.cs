using BoardGameApp.Application.Common;
using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Players;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Infrastructure.Players;

public sealed class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext dbContext;

    public PlayerRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Player> CreateAsync(Player player, CancellationToken cancellationToken = default)
    {
        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(cancellationToken);

        return player;
    }

    public async Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        dbContext.Players.Update(player);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Players
            .FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByFullNameAsync(
        string fullName,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Players.AnyAsync(
            player => player.FullName == fullName
                && (!excludingId.HasValue || player.Id != excludingId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsByWhatsAppAsync(
        string whatsApp,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Players.AnyAsync(
            player => player.WhatsApp == whatsApp
                && (!excludingId.HasValue || player.Id != excludingId.Value),
            cancellationToken);
    }

    public async Task<PagedResult<Player>> ListAsync(
        PlayerFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var currentPage = Math.Max(page, 1);
        var query = dbContext.Players.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(player => player.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(filter.FullName))
        {
            query = query.Where(player => player.FullName.Contains(filter.FullName));
        }

        if (!string.IsNullOrWhiteSpace(filter.WhatsApp))
        {
            query = query.Where(player => player.WhatsApp.Contains(filter.WhatsApp));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(player => player.FullName)
            .Skip((currentPage - 1) * IPlayerRepository.PageSize)
            .Take(IPlayerRepository.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Player>(
            items,
            currentPage,
            IPlayerRepository.PageSize,
            totalCount);
    }
}
