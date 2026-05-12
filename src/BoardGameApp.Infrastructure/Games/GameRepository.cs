using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Domain.Games;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Infrastructure.Games;

public sealed class GameRepository : IGameRepository
{
    private readonly AppDbContext dbContext;

    public GameRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Game> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {
        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync(cancellationToken);

        return game;
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        dbContext.Games.Update(game);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Games
            .FirstOrDefaultAsync(game => game.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAndPublisherAsync(
        string name,
        int publisherId,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Games.AnyAsync(
            game => game.Name == name
                && game.PublisherId == publisherId
                && (!excludingId.HasValue || game.Id != excludingId.Value),
            cancellationToken);
    }

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var game = await GetByIdAsync(id, cancellationToken);

        if (game is null)
        {
            return;
        }

        game.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Game>> ListAsync(
        GameFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var currentPage = Math.Max(page, 1);
        var query = dbContext.Games.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(game => game.IsActive);
        }

        if (filter.Id.HasValue)
        {
            query = query.Where(game => game.Id == filter.Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(game => game.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Author))
        {
            query = query.Where(game => game.Author.Contains(filter.Author));
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(game => game.GenreId == filter.GenreId.Value);
        }

        if (filter.PublisherId.HasValue)
        {
            query = query.Where(game => game.PublisherId == filter.PublisherId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(game => game.Name)
            .Skip((currentPage - 1) * IGameRepository.PageSize)
            .Take(IGameRepository.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Game>(
            items,
            currentPage,
            IGameRepository.PageSize,
            totalCount);
    }
}
