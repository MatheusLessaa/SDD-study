using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Application.Authors;

public interface IAuthorRepository
{
    const int PageSize = 20;

    Task<Author> CreateAsync(Author author, CancellationToken cancellationToken = default);

    Task UpdateAsync(Author author, CancellationToken cancellationToken = default);

    Task DeleteAsync(Author author, CancellationToken cancellationToken = default);

    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsUsedByGameAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<Author>> ListAsync(
        AuthorFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default);
}
