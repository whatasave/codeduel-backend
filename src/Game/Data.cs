namespace Game;

public record Game(
    int Id,
    string UniqueId,
    Challenge.Challenge Challenge,
    int OwnerId,
    bool Ended,
    int ModeId,
    int MaxPlayers,
    int GameDuration,
    string[] AllowedLanguages
) {
    public Game(Entity entity) : this(entity.Id, entity.UniqueId, new(entity.Challenge!), entity.OwnerId, entity.Ended, entity.ModeId, entity.MaxPlayers, entity.GameDuration, entity.AllowedLanguages) { }
}
public record CreateGame(
    string UniqueId,
    int ChallengeId,
    int OwnerId,
    bool Ended,
    int ModeId,
    int MaxPlayers,
    int GameDuration,
    string[] AllowedLanguages,
    List<int> Users
);

public record UpdateSubmission(
    int UserId,
    int LobbyId,
    string Code,
    string Language,
    int TestsPassed,
    DateTime SubmittedAt
);
