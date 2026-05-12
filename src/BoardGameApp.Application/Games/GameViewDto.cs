using BoardGameApp.Domain.Games;

namespace BoardGameApp.Application.Games;

public sealed record GameViewDto(
    int Id,
    string Name,
    int PublisherId,
    int GenreId,
    string Author,
    int TimesPlayed,
    int MaxPlayers,
    bool IsActive)
{
    public static GameViewDto FromEntity(Game game)
    {
        return new GameViewDto(
            game.Id,
            game.Name,
            game.PublisherId,
            game.GenreId,
            game.Author,
            game.TimesPlayed,
            game.MaxPlayers,
            game.IsActive);
    }
}
