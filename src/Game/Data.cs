namespace Game;

public record Game(
    int Id,
    string UniqueId,
    Challenge.Challenge Challenge,
    int OwnerId,
    bool Ended,
    Mode Mode,
    int MaxPlayers,
    int Duration,
    string AllowedLanguages,
    DateTime CreatedAt
) {
    public Game(Entity entity, int testCases) : this(entity.Id, entity.UniqueId, new(entity.Challenge!, testCases), entity.OwnerId, entity.Ended, new Mode(entity.Mode!), entity.MaxPlayers, entity.GameDuration, entity.AllowedLanguages, entity.CreatedAt) { }
}

public record GameWithUserData(
    Game Game,
    UserData UserData
) {
    public GameWithUserData(UserEntity entity, int testCases) : this(new Game(entity.Game!, testCases), new UserData(entity)) { }
}

public record GameWithUsersData(
    Game Game,
    IEnumerable<UserData> UserData
) {
    public GameWithUsersData(Entity entity, int testCases) : this(new Game(entity, testCases), entity.Users!.Select(e => new UserData(e))) { }
}

public record UserData(
    User.UserListItem User,
    string? Code,
    string? Language,
    int TestsPassed,
    DateTime? SubmittedAt,
    bool ShowCode
) {
    public UserData(UserEntity entity) : this(new(entity.User!), entity.Code, entity.Language, entity.TestsPassed, entity.SubmittedAt, entity.ShowCode) { }
}

public record CreateGame(
    string UniqueId,
    int ChallengeId,
    int OwnerId,
    bool Ended,
    int ModeId,
    int MaxPlayers,
    int GameDuration,
    string AllowedLanguages,
    List<int> Users
);

public record UpdateSubmission(
    int UserId,
    int GameId,
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