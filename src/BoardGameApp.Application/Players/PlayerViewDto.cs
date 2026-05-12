using BoardGameApp.Domain.Players;

namespace BoardGameApp.Application.Players;

public sealed record PlayerViewDto(int Id, string FullName, string WhatsApp, bool IsActive)
{
    public static PlayerViewDto FromEntity(Player player)
    {
        return new PlayerViewDto(
            player.Id,
            player.FullName,
            player.WhatsApp,
            player.IsActive);
    }
}
