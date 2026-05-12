using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public sealed record UpdateMatchDto(
    int Id,
    int GameId,
    string PlayerIds,
    string Scores,
    int WinnerPlayerId)
{
    public void ApplyTo(Match match)
    {
        match.GameId = GameId;
        match.PlayerIds = PlayerIds;
        match.Scores = Scores;
        match.WinnerPlayerId = WinnerPlayerId;
    }
}
