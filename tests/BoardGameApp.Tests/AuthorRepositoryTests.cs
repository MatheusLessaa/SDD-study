using BoardGameApp.Application.Authors;
using BoardGameApp.Domain.Authors;
using BoardGameApp.Domain.Games;
using BoardGameApp.Infrastructure.Authors;
using BoardGameApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Tests;

public class AuthorRepositoryTests
{
    [Fact]
    public async Task Create_and_get_by_id_persists_author()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);

        var created = await repository.CreateAsync(new Author { Name = "Reiner Knizia" });

        var found = await repository.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Reiner Knizia", found.Name);
    }

    [Fact]
    public async Task Update_persists_author_changes()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = await repository.CreateAsync(new Author { Name = "Before" });

        author.Name = "After";
        await repository.UpdateAsync(author);

        var updated = await repository.GetByIdAsync(author.Id);

        Assert.NotNull(updated);
        Assert.Equal("After", updated.Name);
    }

    [Fact]
    public async Task Delete_removes_author()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = await repository.CreateAsync(new Author { Name = "To Delete" });

        await repository.DeleteAsync(author);

        Assert.Null(await repository.GetByIdAsync(author.Id));
    }

    [Fact]
    public async Task List_applies_name_filter_and_pagination()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);

        for (var index = 1; index <= 25; index++)
        {
            await repository.CreateAsync(new Author { Name = $"Author {index:00}" });
        }

        var firstPage = await repository.ListAsync(new AuthorFilter("Author"));
        var secondPage = await repository.ListAsync(new AuthorFilter("Author"), page: 2);

        Assert.Equal(20, firstPage.Items.Count);
        Assert.Equal(5, secondPage.Items.Count);
        Assert.Equal(25, firstPage.TotalCount);
        Assert.Equal(20, firstPage.PageSize);
        Assert.Equal("Author 21", secondPage.Items[0].Name);
    }

    [Fact]
    public async Task Exists_by_name_ignores_excluded_author()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = await repository.CreateAsync(new Author { Name = "Same Name" });

        var existsForSameAuthor = await repository.ExistsByNameAsync("Same Name", author.Id);
        var existsForOtherAuthor = await repository.ExistsByNameAsync("Same Name");

        Assert.False(existsForSameAuthor);
        Assert.True(existsForOtherAuthor);
    }

    [Fact]
    public async Task Is_used_by_game_detects_references()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = await repository.CreateAsync(new Author { Name = "Referenced" });
        dbContext.Games.Add(new Game
        {
            Name = "Azul",
            AuthorId = author.Id,
            GenreId = 1,
            PublisherId = 1,
            MaxPlayers = 4
        });
        await dbContext.SaveChangesAsync();

        var isUsed = await repository.IsUsedByGameAsync(author.Id);

        Assert.True(isUsed);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
