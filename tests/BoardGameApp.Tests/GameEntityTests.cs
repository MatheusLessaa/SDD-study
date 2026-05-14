using BoardGameApp.Domain.Games;

namespace BoardGameApp.Tests;

public class GameEntityTests
{
    [Fact]
    public void Game_has_expected_default_values()
    {
        var game = new Game();

        Assert.Equal(0, game.Id);
        Assert.Equal(string.Empty, game.Name);
        Assert.Equal(0, game.PublisherId);
        Assert.Equal(0, game.GenreId);
        Assert.Equal(0, game.AuthorId);
        Assert.Equal(0, game.TimesPlayed);
        Assert.Equal(0, game.MaxPlayers);
        Assert.True(game.IsActive);
    }

    [Fact]
    public void Game_allows_setting_required_spec_fields()
    {
        var game = new Game
        {
            Id = 3,
            Name = "Azul",
            PublisherId = 5,
            GenreId = 8,
            AuthorId = 2,
            TimesPlayed = 12,
            MaxPlayers = 4,
            IsActive = false
        };

        Assert.Equal(3, game.Id);
        Assert.Equal("Azul", game.Name);
        Assert.Equal(5, game.PublisherId);
        Assert.Equal(8, game.GenreId);
        Assert.Equal(2, game.AuthorId);
        Assert.Equal(12, game.TimesPlayed);
        Assert.Equal(4, game.MaxPlayers);
        Assert.False(game.IsActive);
    }
}
