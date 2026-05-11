using BoardGameApp.Domain.Genres;

namespace BoardGameApp.Tests;

public class GenreEntityTests
{
    [Fact]
    public void Genre_has_expected_default_values()
    {
        var genre = new Genre();

        Assert.Equal(0, genre.Id);
        Assert.Equal(string.Empty, genre.Name);
    }

    [Fact]
    public void Genre_allows_setting_required_spec_fields()
    {
        var genre = new Genre
        {
            Id = 2,
            Name = "Strategy"
        };

        Assert.Equal(2, genre.Id);
        Assert.Equal("Strategy", genre.Name);
    }
}
