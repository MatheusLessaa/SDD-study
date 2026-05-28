using BoardGameApp.Domain.Authors;

namespace BoardGameApp.Application.Authors;

public sealed record CreateAuthorDto(string Name)
{
    public Author ToEntity()
    {
        return new Author
        {
            Name = Name.Trim()
        };
    }
}
