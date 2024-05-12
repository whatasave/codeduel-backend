namespace AuthGithub;

public class Service(Repository repository, User.Service userService, Jwt.Service jwtService) {

    public Service(DatabaseContext database) : this(
        new Repository(database),
        new User.Service(database),
        new Jwt.Service(database)
    ) {}

    public Entity FindById(int id) {
        return repository.GetAuthByProviderAndId("github", id);
    }
    public User.User? GetUserByProviderAndId(string provider, int providerId) {
        var authUser = repository.GetAuthByProviderAndId(provider, providerId);

        if (authUser == null) return null;

        return userService.FindById(authUser.UserId);
    }

    public User.User CreateAuthUser(GithubUserData user) {
        var newUser = userService.Create(new (-1, user.Login) {
            Name = user.Name ?? user.Login,
            Avatar = user.AvatarUrl
        });

        repository.Create(new Entity {
            Id = -1,
            UserId = newUser.Id,
            Provider = "github",
            ProviderId = user.Id
        });

        return newUser;
    }

    public Entity AuthenticateWithGithub(int id) {
        return repository.FindById(id);
    }
    public string GetAccessToken(string code, string state) {
        return "123456";
    }
    public GithubUserData GetUserData(string accessToken) {
        return new GithubUserData();
    }
    public List<GithubEmail> GetUserEmails(string accessToken) {
        return [
            new("test1@gh.com",true,true,"private"),
            new("test2@gh.com",true,false,"private"),
            new("test3@gh.com",true,false,"public")
        ];
    }
    public string? GetUserPrimaryEmail(string accessToken) {
        var emails = GetUserEmails(accessToken);
        var primaryEmail = emails.Find(static e => e.Verified && e.Primary);

        return primaryEmail?.Email;
    }

    public string[] GenerateTokens(User.User user) {
        var refreshToken = jwtService.GenerateRefreshToken(user);
        var accessToken = jwtService.GenerateAccessToken(user);

        jwtService.SaveRefreshToken(user.Id, refreshToken);

        return [accessToken, refreshToken];
    }
} 