namespace Challenge;

public record Challenge(
    int Id,
    User.UserListItem Owner,
    string Title,
    string Description,
    string Content,
    int TestCases,
    DateTime CreatedAt
) {
    public Challenge(Entity entity, int testCases) : this(
        entity.Id,
        new(entity.Owner!),
        entity.Title,
        entity.Description,
        entity.Content,
        testCases,
        entity.CreatedAt
    ) { }
}

public record ChallengeDetailed(
    int Id,
    User.UserListItem Owner,
    string Title,
    string Description,
    string Content,
    TestCase[] TestCases,
    TestCase[] HiddenTestCases,
    DateTime CreatedAt
) {
    public ChallengeDetailed(Entity entity) : this(
        entity.Id,
        new(entity.Owner!),
        entity.Title,
        entity.Description,
        entity.Content,
        entity.TestCases!.Where(tc => !tc.Hidden).Select(tc => new TestCase(tc)).ToArray(),
        entity.TestCases!.Where(tc => tc.Hidden).Select(tc => new TestCase(tc)).ToArray(),
        entity.CreatedAt
    ) { }
}

public record TestCase(
    string Input,
    string Output
) {
    public TestCase(TestCaseEntity entity) : this(
        entity.Input,
        entity.Output
    ) { }
}

public record CreateChallenge(
    string Title,
    string Description,
    string Content
);
