using BoardGameApp.Domain.Players;

namespace BoardGameApp.Tests;

public class PlayerEntityTests
{
    [Fact]
    public void Player_has_expected_default_values()
    {
        var player = new Player();

        Assert.Equal(0, player.Id);
        Assert.Equal(string.Empty, player.FullName);
        Assert.Equal(string.Empty, player.WhatsApp);
        Assert.True(player.IsActive);
    }

    [Fact]
    public void Player_allows_setting_required_spec_fields()
    {
        var player = new Player
        {
            Id = 7,
            FullName = "Ada Lovelace",
            WhatsApp = "+5511999999999",
            IsActive = false
        };

        Assert.Equal(7, player.Id);
        Assert.Equal("Ada Lovelace", player.FullName);
        Assert.Equal("+5511999999999", player.WhatsApp);
        Assert.False(player.IsActive);
    }
}
