using BoardGameApp.Domain.Common;

namespace BoardGameApp.Domain.Matches;

public class Match : Entity
{
    public int GameId { get; set; }

    public string PlayerIds { get; set; } = string.Empty;

    public string Scores { get; set; } = string.Empty;

    public int WinnerPlayerId { get; set; }
}
