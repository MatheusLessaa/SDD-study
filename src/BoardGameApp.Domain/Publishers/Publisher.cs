using BoardGameApp.Domain.Common;

namespace BoardGameApp.Domain.Publishers;

public class Publisher : Entity
{
    public string Name { get; set; } = string.Empty;
}
