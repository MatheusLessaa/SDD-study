using BoardGameApp.Domain.Common;

namespace BoardGameApp.Domain.Games;

public class Game : Entity
{
    public string Name { get; set; } = string.Empty;

    public int PublisherId { get; set; }

    public int GenreId { get; set; }

    public int AuthorId { get; set; }

    public int TimesPlayed { get; set; }

    public int MaxPlayers { get; set; }

    public bool IsActive { get; set; } = true;
}
