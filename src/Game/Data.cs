namespace Game;

public record Game(
    int Id,
    string Uuid,
    Challenge.Challenge Challenge,
    int OwnerId,
    bool Ended,
    string Mode,
    int MaxPlayers,
    int GameDuration,
    string AllowedLanguages
);