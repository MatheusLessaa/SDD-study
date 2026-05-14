using BoardGameApp.Domain.Games;

namespace BoardGameApp.Application.Games;

public sealed record CreateGameDto(
    string Name,
    int PublisherId,
    int GenreId,
    int AuthorId,
    int MaxPlayers)
{
    public Game ToEntity()
    {
        return new Game
        {
            Name = Name,
            PublisherId = PublisherId,
            GenreId = GenreId,
            AuthorId = AuthorId,
            MaxPlayers = MaxPlayers
        };
    }
}
