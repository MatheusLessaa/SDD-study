using BoardGameApp.Application.Players;
using BoardGameApp.Domain.Players;

namespace BoardGameApp.Tests;

public class PlayerDtoTests
{
    [Fact]
    public void Create_player_dto_maps_to_entity_without_exposing_entity_contract()
    {
        var dto = new CreatePlayerDto("Ada Lovelace", "1111");

        var player = dto.ToEntity();

        Assert.Equal("Ada Lovelace", player.FullName);
        Assert.Equal("1111", player.WhatsApp);
        Assert.True(player.IsActive);
    }

    [Fact]
    public void Update_player_dto_applies_changes_to_entity()
    {
        var player = new Player
        {
            Id = 7,
            FullName = "Before",
            WhatsApp = "2222"
        };
        var dto = new UpdatePlayerDto(7, "After", "3333", false);

        dto.ApplyTo(player);

        Assert.Equal(7, player.Id);
        Assert.Equal("After", player.FullName);
        Assert.Equal("3333", player.WhatsApp);
        Assert.False(player.IsActive);
    }

    [Fact]
    public void Player_view_dto_maps_from_entity()
    {
        var player = new Player
        {
            Id = 5,
            FullName = "Grace Hopper",
            WhatsApp = "4444",
            IsActive = false
        };

        var dto = PlayerViewDto.FromEntity(player);

        Assert.Equal(5, dto.Id);
        Assert.Equal("Grace Hopper", dto.FullName);
        Assert.Equal("4444", dto.WhatsApp);
        Assert.False(dto.IsActive);
    }
}
