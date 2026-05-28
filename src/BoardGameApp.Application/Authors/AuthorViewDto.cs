using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Application.Authors;

public sealed record AuthorViewDto(int Id, string Name)
{
    public static AuthorViewDto FromEntity(Author author)
    {
        return new AuthorViewDto(author.Id, author.Name);
    }
}
