using BoardGameApp.Application.Games;
using BoardGameApp.Domain.Games;

namespace BoardGameApp.Tests;

public class GameDtoTests
{
    [Fact]
    public void Create_game_dto_maps_to_entity()
    {
        var dto = new CreateGameDto(
            "Azul",
            PublisherId: 1,
            GenreId: 2,
            "Michael Kiesling",
            MaxPlayers: 4);

        var game = dto.ToEntity();

        Assert.Equal("Azul", game.Name);
        Assert.Equal(1, game.PublisherId);
        Assert.Equal(2, game.GenreId);
        Assert.Equal("Michael Kiesling", game.Author);
        Assert.Equal(0, game.TimesPlayed);
        Assert.Equal(4, game.MaxPlayers);
        Assert.True(game.IsActive);
    }

    [Fact]
    public void Update_game_dto_applies_changes_to_entity()
    {
        var game = new Game
        {
            Id = 8,
            Name = "Before",
            PublisherId = 1,
            GenreId = 1,
            Author = "Before Author",
            TimesPlayed = 1,
            MaxPlayers = 2
        };
        var dto = new UpdateGameDto(
            8,
            "After",
            PublisherId: 2,
            GenreId: 3,
            "After Author",
            TimesPlayed: 9,
            MaxPlayers: 5,
            IsActive: false);

        dto.ApplyTo(game);

        Assert.Equal(8, game.Id);
        Assert.Equal("After", game.Name);
        Assert.Equal(2, game.PublisherId);
        Assert.Equal(3, game.GenreId);
        Assert.Equal("After Author", game.Author);
        Assert.Equal(9, game.TimesPlayed);
        Assert.Equal(5, game.MaxPlayers);
        Assert.False(game.IsActive);
    }

    [Fact]
    public void Game_view_dto_maps_from_entity()
    {
        var game = new Game
        {
            Id = 6,
            Name = "Catan",
            PublisherId = 2,
            GenreId = 1,
            Author = "Klaus Teuber",
            TimesPlayed = 15,
            MaxPlayers = 4,
            IsActive = false
        };

        var dto = GameViewDto.FromEntity(game);

        Assert.Equal(6, dto.Id);
        Assert.Equal("Catan", dto.Name);
        Assert.Equal(2, dto.PublisherId);
        Assert.Equal(1, dto.GenreId);
        Assert.Equal("Klaus Teuber", dto.Author);
        Assert.Equal(15, dto.TimesPlayed);
        Assert.Equal(4, dto.MaxPlayers);
        Assert.False(dto.IsActive);
    }
}
