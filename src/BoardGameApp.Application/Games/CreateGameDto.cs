using BoardGameApp.Domain.Games;

namespace BoardGameApp.Application.Games;

public sealed record CreateGameDto(
    string Name,
    int PublisherId,
    int GenreId,
    string Author,
    int MaxPlayers)
{
    public Game ToEntity()
    {
        return new Game
        {
            Name = Name,
            PublisherId = PublisherId,
            GenreId = GenreId,
            Author = Author,
            MaxPlayers = MaxPlayers
        };
    }
}
