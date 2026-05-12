using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Tests;

public class MatchEntityTests
{
    [Fact]
    public void Match_has_expected_default_values()
    {
        var match = new Match();

        Assert.Equal(0, match.Id);
        Assert.Equal(0, match.GameId);
        Assert.Equal(string.Empty, match.PlayerIds);
        Assert.Equal(string.Empty, match.Scores);
        Assert.Equal(0, match.WinnerPlayerId);
        Assert.Equal(default, match.CreatedAt);
    }

    [Fact]
    public void Match_allows_setting_required_spec_fields()
    {
        var match = new Match
        {
            Id = 11,
            GameId = 4,
            PlayerIds = "1,5,8",
            Scores = "10,7,3",
            WinnerPlayerId = 1,
            CreatedAt = new DateTime(2026, 5, 12, 18, 30, 0)
        };

        Assert.Equal(11, match.Id);
        Assert.Equal(4, match.GameId);
        Assert.Equal("1,5,8", match.PlayerIds);
        Assert.Equal("10,7,3", match.Scores);
        Assert.Equal(1, match.WinnerPlayerId);
        Assert.Equal(new DateTime(2026, 5, 12, 18, 30, 0), match.CreatedAt);
    }
}
