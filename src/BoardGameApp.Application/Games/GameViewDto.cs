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
    bool IsActive,
    string PublisherName = "",
    string GenreName = "")
{
    private static readonly IReadOnlyDictionary<int, string> PublisherNames = new Dictionary<int, string>
    {
        [1] = "Galapagos",
        [2] = "Devir",
        [3] = "Meeple BR"
    };

    private static readonly IReadOnlyDictionary<int, string> GenreNames = new Dictionary<int, string>
    {
        [1] = "Strategy",
        [2] = "Family",
        [3] = "Party"
    };

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
            game.IsActive,
            ResolveName(PublisherNames, game.PublisherId),
            ResolveName(GenreNames, game.GenreId));
    }

    private static string ResolveName(IReadOnlyDictionary<int, string> names, int id)
    {
        return names.TryGetValue(id, out var name)
            ? name
            : $"#{id}";
    }
}
