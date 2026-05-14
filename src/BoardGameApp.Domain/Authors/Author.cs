using BoardGameApp.Domain.Common;

namespace BoardGameApp.Domain.Authors;

public class Author : Entity
{
    public string Name { get; set; } = string.Empty;
}
