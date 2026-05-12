using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;

namespace BoardGameApp.Web.Areas.Matches.Models;

public sealed class MatchCreateViewModel
{
    public int GameId { get; init; }

    public string PlayerIds { get; init; } = string.Empty;

    public string Scores { get; init; } = string.Empty;

    public int WinnerPlayerId { get; init; }

    public IReadOnlyList<GameViewDto> Games { get; init; } = [];

    public IReadOnlyList<PlayerViewDto> Players { get; init; } = [];

    public CreateMatchDto ToDto()
    {
        return new CreateMatchDto(GameId, PlayerIds, Scores, WinnerPlayerId);
    }
}
