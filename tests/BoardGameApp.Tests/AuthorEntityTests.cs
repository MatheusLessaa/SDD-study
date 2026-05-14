using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Tests;

public class AuthorEntityTests
{
    [Fact]
    public void Author_has_expected_default_values()
    {
        var author = new Author();

        Assert.Equal(0, author.Id);
        Assert.Equal(string.Empty, author.Name);
    }

    [Fact]
    public void Author_allows_setting_required_spec_fields()
    {
        var author = new Author
        {
            Id = 1,
            Name = "Michael Kiesling"
        };

        Assert.Equal(1, author.Id);
        Assert.Equal("Michael Kiesling", author.Name);
    }
}
