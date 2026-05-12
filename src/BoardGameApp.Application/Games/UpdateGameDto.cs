using BoardGameApp.Domain.Games;

namespace BoardGameApp.Application.Games;

public sealed record UpdateGameDto(
    int Id,
    string Name,
    int PublisherId,
    int GenreId,
    string Author,
    int TimesPlayed,
    int MaxPlayers,
    bool IsActive)
{
    public void ApplyTo(Game game)
    {
        game.Name = Name;
        game.PublisherId = PublisherId;
        game.GenreId = GenreId;
        game.Author = Author;
        game.TimesPlayed = TimesPlayed;
        game.MaxPlayers = MaxPlayers;
        game.IsActive = IsActive;
    }
}
