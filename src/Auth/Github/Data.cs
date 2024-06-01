using System.Text.Json.Serialization;

namespace Auth.Github;

public record GithubUserData(
    string Login,
    int Id,
    string NodeId,
    string AvatarUrl,
    string GravatarId,
    string Url,
    string HtmlUrl,
    string FollowersUrl,
    string FollowingUrl,
    string GistsUrl,
    string StarredUrl,
    string SubscriptionsUrl,
    string OrganizationsUrl,
    string ReposUrl,
    string EventsUrl,
    string ReceivedEventsUrl,
    string Type,
    bool? SiteAdmin,
    string Name,
    object Company,
    string Blog,
    string Location,
    object Email,
    object Hireable,
    object Bio,
    object TwitterUsername,
    int PublicRepos,
    int PublicGists,
    int Followers,
    int Following,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
) {
    public bool IsEmpty => string.IsNullOrEmpty(Login);
};

public record GithubEmail(
    string Email,
    bool Verified,
    bool Primary,
    string Visibility
) {
    public bool IsEmpty => string.IsNullOrEmpty(Email);
};

public record GithubAccessToken(
    string AccessToken,
    string TokenType,
    string Scope
) {
    public bool IsEmpty => string.IsNullOrEmpty(AccessToken);
};
