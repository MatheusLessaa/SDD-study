using BoardGameApp.Application.Matches;

namespace BoardGameApp.Web.Areas.Matches.Models;

public sealed class MatchEditScoresViewModel
{
    public required MatchViewDto Match { get; init; }

    public UpdateMatchScoresDto ScoreUpdate => new(Match.Id, Match.Scores);
}
