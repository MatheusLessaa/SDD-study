using BoardGameApp.Application.Games;

namespace BoardGameApp.Web.Areas.Games.Models;

public sealed class GameEditViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int PublisherId { get; set; }

    public int GenreId { get; set; }

    public int AuthorId { get; set; }

    public int TimesPlayed { get; set; }

    public int MaxPlayers { get; set; }

    public bool IsActive { get; set; } = true;

    public IReadOnlyList<PublisherOptionDto> PublisherOptions { get; init; } = [];

    public UpdateGameDto ToDto()
    {
        return new UpdateGameDto(Id, Name, PublisherId, GenreId, AuthorId, TimesPlayed, MaxPlayers, IsActive);
    }
}
