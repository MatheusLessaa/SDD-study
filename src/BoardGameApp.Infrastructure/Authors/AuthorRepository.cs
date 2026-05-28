using BoardGameApp.Application.Authors;
using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Authors;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Infrastructure.Authors;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly AppDbContext dbContext;

    public AuthorRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Author> CreateAsync(Author author, CancellationToken cancellationToken = default)
    {
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync(cancellationToken);

        return author;
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        dbContext.Authors.Update(author);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
    {
        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Authors
            .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Authors.AnyAsync(
            author => author.Name == name
                && (!excludingId.HasValue || author.Id != excludingId.Value),
            cancellationToken);
    }

    public Task<bool> IsUsedByGameAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Games.AnyAsync(game => game.AuthorId == id, cancellationToken);
    }

    public async Task<PagedResult<Author>> ListAsync(
        AuthorFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var currentPage = Math.Max(page, 1);
        var query = dbContext.Authors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(author => author.Name.Contains(filter.Name));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(author => author.Name)
            .Skip((currentPage - 1) * IAuthorRepository.PageSize)
            .Take(IAuthorRepository.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Author>(
            items,
            currentPage,
            IAuthorRepository.PageSize,
            totalCount);
    }
}
