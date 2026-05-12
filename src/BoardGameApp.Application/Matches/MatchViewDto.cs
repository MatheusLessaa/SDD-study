using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public sealed record MatchViewDto(
    int Id,
    int GameId,
    string PlayerIds,
    string Scores,
    int WinnerPlayerId)
{
    public static MatchViewDto FromEntity(Match match)
    {
        return new MatchViewDto(
            match.Id,
            match.GameId,
            match.PlayerIds,
            match.Scores,
            match.WinnerPlayerId);
    }
}
