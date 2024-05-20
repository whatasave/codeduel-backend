namespace Auth.Github;

public record GithubUserData(
    string Login = "",
    int Id = 0,
    string NodeId = "",
    string AvatarUrl = "",
    string GravatarId = "",
    string Url = "",
    string HtmlUrl = "",
    string FollowersUrl = "",
    string FollowingUrl = "",
    string GistsUrl = "",
    string StarredUrl = "",
    string SubscriptionsUrl = "",
    string OrganizationsUrl = "",
    string ReposUrl = "",
    string EventsUrl = "",
    string ReceivedEventsUrl = "",
    string Type = "",
    bool SiteAdmin = false,
    string Name = "",
    string Company = "",
    string Blog = "",
    string Location = "",
    string Email = "",
    string Hireable = "",
    string Bio = "",
    int PublicRepos = 0,
    int PublicGists = 0,
    int Followers = 0,
    int Following = 0,
    DateTime? CreatedAt = null,
    DateTime? UpdatedAt = null
);

public record GithubEmail(
    string Email,
    bool Verified,
    bool Primary,
    string Visibility
);

public record GithubAccessToken(
    string AccessToken,
    string TokenType,
    string Scope
);
