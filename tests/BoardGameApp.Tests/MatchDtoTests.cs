using BoardGameApp.Application.Matches;
using BoardGameApp.Domain.Matches;

namespace BoardGameApp.Tests;

public class MatchDtoTests
{
    [Fact]
    public void Create_match_dto_maps_to_entity()
    {
        var dto = new CreateMatchDto(
            GameId: 4,
            PlayerIds: "1,5,8",
            Scores: "10,7,3",
            WinnerPlayerId: 1);

        var match = dto.ToEntity();

        Assert.Equal(4, match.GameId);
        Assert.Equal("1,5,8", match.PlayerIds);
        Assert.Equal("10,7,3", match.Scores);
        Assert.Equal(1, match.WinnerPlayerId);
    }

    [Fact]
    public void Update_match_dto_applies_changes_to_entity()
    {
        var match = new Match
        {
            Id = 9,
            GameId = 1,
            PlayerIds = "1,2",
            Scores = "4,5",
            WinnerPlayerId = 2
        };
        var dto = new UpdateMatchDto(
            9,
            GameId: 3,
            PlayerIds: "4,5,6",
            Scores: "8,10,2",
            WinnerPlayerId: 5);

        dto.ApplyTo(match);

        Assert.Equal(9, match.Id);
        Assert.Equal(3, match.GameId);
        Assert.Equal("4,5,6", match.PlayerIds);
        Assert.Equal("8,10,2", match.Scores);
        Assert.Equal(5, match.WinnerPlayerId);
    }

    [Fact]
    public void Match_view_dto_maps_from_entity()
    {
        var match = new Match
        {
            Id = 12,
            GameId = 6,
            PlayerIds = "7,8",
            Scores = "11,9",
            WinnerPlayerId = 7
        };

        var dto = MatchViewDto.FromEntity(match);

        Assert.Equal(12, dto.Id);
        Assert.Equal(6, dto.GameId);
        Assert.Equal("7,8", dto.PlayerIds);
        Assert.Equal("11,9", dto.Scores);
        Assert.Equal(7, dto.WinnerPlayerId);
    }
}
