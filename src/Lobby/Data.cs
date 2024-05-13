namespace Lobby;

public record Lobby(
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