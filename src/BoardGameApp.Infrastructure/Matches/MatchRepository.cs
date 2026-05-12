using BoardGameApp.Application.Common;
using BoardGameApp.Application.Matches;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Infrastructure.Matches;

public sealed class MatchRepository : IMatchRepository
{
    private readonly AppDbContext dbContext;

    public MatchRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default)
    {
        dbContext.Matches.Add(match);
        await dbContext.SaveChangesAsync(cancellationToken);

        return match;
    }

    public async Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        dbContext.Matches.Update(match);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Matches
            .FirstOrDefaultAsync(match => match.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Match>> ListAsync(
        MatchFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var currentPage = Math.Max(page, 1);
        var query = dbContext.Matches.AsQueryable();

        if (filter.Id.HasValue)
        {
            query = query.Where(match => match.Id == filter.Id.Value);
        }

        if (filter.GameId.HasValue)
        {
            query = query.Where(match => match.GameId == filter.GameId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(match => match.Id)
            .Skip((currentPage - 1) * IMatchRepository.PageSize)
            .Take(IMatchRepository.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Match>(
            items,
            currentPage,
            IMatchRepository.PageSize,
            totalCount);
    }
}
