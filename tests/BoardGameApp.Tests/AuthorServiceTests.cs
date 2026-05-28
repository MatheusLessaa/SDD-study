using BoardGameApp.Application.Authors;
using BoardGameApp.Application.Common;
using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Tests;

public class AuthorServiceTests
{
    [Fact]
    public async Task Create_enforces_required_name()
    {
        var service = new AuthorService(new FakeAuthorRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateAuthorDto("   ")));

        Assert.Contains("required", exception.Message);
    }

    [Fact]
    public async Task Create_enforces_unique_name()
    {
        var repository = new FakeAuthorRepository();
        await repository.CreateAsync(new Author { Name = "Michael Kiesling" });
        var service = new AuthorService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreateAuthorDto("Michael Kiesling")));

        Assert.Contains("already in use", exception.Message);
    }

    [Fact]
    public async Task Create_trims_name_and_returns_view_dto()
    {
        var service = new AuthorService(new FakeAuthorRepository());

        var created = await service.CreateAsync(new CreateAuthorDto("  Reiner Knizia  "));

        Assert.Equal(1, created.Id);
        Assert.Equal("Reiner Knizia", created.Name);
    }

    [Fact]
    public async Task Update_allows_same_author_to_keep_name()
    {
        var repository = new FakeAuthorRepository();
        var author = await repository.CreateAsync(new Author { Name = "Before" });
        var service = new AuthorService(repository);

        var updated = await service.UpdateAsync(new UpdateAuthorDto(author.Id, "Before"));

        Assert.Equal(author.Id, updated.Id);
        Assert.Equal("Before", updated.Name);
    }

    [Fact]
    public async Task Update_enforces_unique_name_against_other_authors()
    {
        var repository = new FakeAuthorRepository();
        await repository.CreateAsync(new Author { Name = "Existing" });
        var target = await repository.CreateAsync(new Author { Name = "Target" });
        var service = new AuthorService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAsync(new UpdateAuthorDto(target.Id, "Existing")));

        Assert.Contains("already in use", exception.Message);
    }

    [Fact]
    public async Task Delete_removes_author_when_unused()
    {
        var repository = new FakeAuthorRepository();
        var author = await repository.CreateAsync(new Author { Name = "Unused" });
        var service = new AuthorService(repository);

        await service.DeleteAsync(author.Id);

        Assert.Null(await repository.GetByIdAsync(author.Id));
    }

    [Fact]
    public async Task Delete_rejects_author_used_by_games()
    {
        var repository = new FakeAuthorRepository();
        var author = await repository.CreateAsync(new Author { Name = "Used" });
        repository.UsedAuthorIds.Add(author.Id);
        var service = new AuthorService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DeleteAsync(author.Id));

        Assert.Contains("used by one or more games", exception.Message);
        Assert.NotNull(await repository.GetByIdAsync(author.Id));
    }

    [Fact]
    public async Task List_maps_repository_entities_to_view_dtos()
    {
        var repository = new FakeAuthorRepository();
        await repository.CreateAsync(new Author { Name = "Ada Author" });
        var service = new AuthorService(repository);

        var result = await service.ListAsync(new AuthorFilter());

        Assert.Single(result.Items);
        Assert.IsType<AuthorViewDto>(result.Items[0]);
        Assert.Equal("Ada Author", result.Items[0].Name);
    }

    private sealed class FakeAuthorRepository : IAuthorRepository
    {
        private readonly List<Author> authors = [];
        private int nextId = 1;

        public List<int> UsedAuthorIds { get; } = [];

        public Task<Author> CreateAsync(Author author, CancellationToken cancellationToken = default)
        {
            author.Id = nextId++;
            authors.Add(author);

            return Task.FromResult(author);
        }

        public Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
        {
            var index = authors.FindIndex(storedAuthor => storedAuthor.Id == author.Id);

            if (index >= 0)
            {
                authors[index] = author;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
        {
            authors.RemoveAll(storedAuthor => storedAuthor.Id == author.Id);

            return Task.CompletedTask;
        }

        public Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(authors.FirstOrDefault(author => author.Id == id));
        }

        public Task<bool> ExistsByNameAsync(
            string name,
            int? excludingId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(authors.Any(author =>
                author.Name == name
                && (!excludingId.HasValue || author.Id != excludingId.Value)));
        }

        public Task<bool> IsUsedByGameAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UsedAuthorIds.Contains(id));
        }

        public Task<PagedResult<Author>> ListAsync(
            AuthorFilter filter,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var query = authors.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(author => author.Name.Contains(filter.Name));
            }

            var items = query
                .OrderBy(author => author.Name)
                .Skip((Math.Max(page, 1) - 1) * IAuthorRepository.PageSize)
                .Take(IAuthorRepository.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Author>(
                items,
                Math.Max(page, 1),
                IAuthorRepository.PageSize,
                query.Count()));
        }
    }
}
