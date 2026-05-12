using BoardGameApp.Domain.Players;

namespace BoardGameApp.Application.Players;

public sealed record UpdatePlayerDto(int Id, string FullName, string WhatsApp, bool IsActive)
{
    public void ApplyTo(Player player)
    {
        player.FullName = FullName;
        player.WhatsApp = WhatsApp;
        player.IsActive = IsActive;
    }
}
