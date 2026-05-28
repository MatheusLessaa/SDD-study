using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Authors;

public sealed class AuthorService : IAuthorService
{
    private readonly IAuthorRepository authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        this.authorRepository = authorRepository;
    }

    public async Task<AuthorViewDto> CreateAsync(
        CreateAuthorDto dto,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeAndValidateName(dto.Name);
        await EnsureUniqueNameAsync(name, excludingId: null, cancellationToken);

        var author = await authorRepository.CreateAsync(new CreateAuthorDto(name).ToEntity(), cancellationToken);

        return AuthorViewDto.FromEntity(author);
    }

    public async Task<AuthorViewDto> UpdateAsync(
        UpdateAuthorDto dto,
        CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Author with id '{dto.Id}' was not found.");

        var name = NormalizeAndValidateName(dto.Name);
        await EnsureUniqueNameAsync(name, dto.Id, cancellationToken);

        new UpdateAuthorDto(dto.Id, name).ApplyTo(author);
        await authorRepository.UpdateAsync(author, cancellationToken);

        return AuthorViewDto.FromEntity(author);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Author with id '{id}' was not found.");

        if (await authorRepository.IsUsedByGameAsync(id, cancellationToken))
        {
            throw new InvalidOperationException("Author cannot be deleted because it is used by one or more games.");
        }

        await authorRepository.DeleteAsync(author, cancellationToken);
    }

    public async Task<AuthorViewDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken);

        return author is null ? null : AuthorViewDto.FromEntity(author);
    }

    public async Task<PagedResult<AuthorViewDto>> ListAsync(
        AuthorFilter filter,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var authors = await authorRepository.ListAsync(filter, page, cancellationToken);
        var items = authors.Items
            .Select(AuthorViewDto.FromEntity)
            .ToList();

        return new PagedResult<AuthorViewDto>(
            items,
            authors.Page,
            authors.PageSize,
            authors.TotalCount);
    }

    private async Task EnsureUniqueNameAsync(
        string name,
        int? excludingId,
        CancellationToken cancellationToken)
    {
        if (await authorRepository.ExistsByNameAsync(name, excludingId, cancellationToken))
        {
            throw new InvalidOperationException($"Author name '{name}' is already in use.");
        }
    }

    private static string NormalizeAndValidateName(string name)
    {
        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new InvalidOperationException("Author name is required.");
        }

        return normalizedName;
    }
}
