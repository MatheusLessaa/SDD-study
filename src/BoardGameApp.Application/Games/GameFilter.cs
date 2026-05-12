namespace BoardGameApp.Application.Games;

public sealed record GameFilter(
    int? Id = null,
    string? Name = null,
    string? Author = null,
    int? GenreId = null,
    int? PublisherId = null);
