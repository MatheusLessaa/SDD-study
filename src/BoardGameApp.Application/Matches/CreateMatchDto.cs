using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public sealed record CreateMatchDto(
    int GameId,
    string PlayerIds,
    string Scores,
    int WinnerPlayerId)
{
    public Match ToEntity()
    {
        return new Match
        {
            GameId = GameId,
            PlayerIds = PlayerIds,
            Scores = Scores,
            WinnerPlayerId = WinnerPlayerId,
            CreatedAt = DateTime.Now
        };
    }
}
