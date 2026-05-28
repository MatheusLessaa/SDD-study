using BoardGameApp.Application.Games;

namespace BoardGameApp.Web.Areas.Games.Models;

public sealed class GameCreateViewModel
{
    public string Name { get; set; } = string.Empty;

    public int PublisherId { get; set; }

    public int GenreId { get; set; }

    public int AuthorId { get; set; }

    public int MaxPlayers { get; set; } = 1;

    public IReadOnlyList<GenreOptionDto> GenreOptions { get; init; } = [];

    public IReadOnlyList<PublisherOptionDto> PublisherOptions { get; init; } = [];

    public CreateGameDto ToDto()
    {
        return new CreateGameDto(Name, PublisherId, GenreId, AuthorId, MaxPlayers);
    }
}
