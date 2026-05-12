using BoardGameApp.Domain.Players;

namespace BoardGameApp.Application.Players;

public sealed record CreatePlayerDto(string FullName, string WhatsApp)
{
    public Player ToEntity()
    {
        return new Player
        {
            FullName = FullName,
            WhatsApp = WhatsApp
        };
    }
}
