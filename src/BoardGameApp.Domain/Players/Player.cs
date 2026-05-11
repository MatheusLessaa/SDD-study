using BoardGameApp.Domain.Common;

namespace BoardGameApp.Domain.Players;

public class Player : Entity
{
    public string FullName { get; set; } = string.Empty;

    public string WhatsApp { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
