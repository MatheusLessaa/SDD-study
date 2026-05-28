using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Application.Matches;

public sealed record MatchViewDto(
    int Id,
    int GameId,
    string PlayerIds,
    string Scores,
    int WinnerPlayerId,
    DateTime CreatedAt,
    string GameName = "",
    string PlayerNames = "",
    string WinnerPlayerName = "",
    IReadOnlyList<MatchPlayerScoreDto>? PlayerScores = null)
{
    public string CreatedDateDisplay => CreatedAt.ToString("dd/MM/yyyy");

    public IReadOnlyList<MatchPlayerScoreDto> PlayerScoreDetails => PlayerScores ?? [];

    public static MatchViewDto FromEntity(Match match)
    {
        return new MatchViewDto(
            match.Id,
            match.GameId,
            match.PlayerIds,
            match.Scores,
            match.WinnerPlayerId,
            match.CreatedAt);
    }
}
