namespace Game;

public record Game(
    int Id,
    string UniqueId,
    Challenge.Challenge Challenge,
    int OwnerId,
    bool Ended,
    Mode Mode,
    int MaxPlayers,
    int GameDuration,
    string[] AllowedLanguages,
    DateTime CreatedAt
) {
    public Game(Entity entity) : this(entity.Id, entity.UniqueId, new(entity.Challenge!), entity.OwnerId, entity.Ended, new Mode(entity.Mode!), entity.MaxPlayers, entity.GameDuration, entity.AllowedLanguages, entity.CreatedAt) { }
}

public record GameWithUserData(
    Game Game,
    UserData UserData
) {
    public GameWithUserData(UserEntity userEntity) : this(new Game(userEntity.Lobby!), new UserData(userEntity)) { }
}

public record GameWithUsersData(
    Game Game,
    IEnumerable<UserData> UserData
) {
    public GameWithUsersData(Entity entity, IEnumerable<UserEntity> userEntities) : this(new Game(entity), userEntities.Select(e => new UserData(e))) { }
}

public record UserData(
    int UserId,
    string Username,
    string Name,
    string? Avatar,
    string? Code,
    string? Language,
    int TestsPassed,
    DateTime? SubmittedAt,
    bool ShowCode
) {
    public UserData(UserEntity entity) : this(entity.User!.Id, entity.User!.Username, entity.User!.Name, entity.User!.Avatar, entity.Code, entity.Language, entity.TestsPassed, entity.SubmittedAt, entity.ShowCode) { }
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

public record ShareCodeRequest(
    int LobbyId,
    bool ShowCode
);

public record Mode(
    int Id,
    string Name,
    string Description
) {
    public Mode(ModeEntity entity) : this(entity.Id, entity.Name, entity.Description) { }
}