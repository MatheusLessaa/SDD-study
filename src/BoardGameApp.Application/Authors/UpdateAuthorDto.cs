using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Application.Authors;

public sealed record UpdateAuthorDto(int Id, string Name)
{
    public void ApplyTo(Author author)
    {
        author.Name = Name.Trim();
    }
}
