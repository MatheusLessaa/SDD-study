namespace BoardGameApp.Application.Matches;

public sealed record MatchPlayerScoreDto(
    int PlayerId,
    string PlayerName,
    int Score);
