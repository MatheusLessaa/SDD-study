using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Authors;

public interface IAuthorService
{
    Task<AuthorViewDto> CreateAsync(
        CreateAuthorDto dto,
        CancellationToken cancellationToken = default);

    Task<AuthorViewDto> UpdateAsync(
        UpdateAuthorDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<AuthorViewDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuthorViewDto>> ListAsync(
        AuthorFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default);
}
