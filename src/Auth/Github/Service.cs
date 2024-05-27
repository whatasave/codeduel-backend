using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Auth.Github;

public class Service(Repository repository, Config.Config config, User.Service userService) {
    public const string PROVIDER = "github";

    public Service(Config.Config config, Database.DatabaseContext database) : this(
        new Repository(database),
        config,
        new User.Service(database)
    ) { }

    public Entity? FindById(int providerId) {
        return repository.GetAuthByProviderAndId(PROVIDER, providerId);
    }

    public User.User? GetUserByProviderId(int providerId) {
        var authUser = repository.GetAuthByProviderAndId(PROVIDER, providerId);
        if (authUser == null) return null;

        return userService.FindById(authUser.UserId);
    }

    public User.User Create(GithubUserData user) {
        var newUser = userService.Create(new(user.Login, user.Name ?? user.Login) {
            Avatar = user.AvatarUrl
        });

        repository.Create(new(
            newUser.Id,
            PROVIDER,
            user.Id
        ));

        return newUser;
    }

    public async Task<GithubAccessToken?> GetAccessToken(string code, string state) {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("codeduel.it", "1.0"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(JsonSerializer.Serialize(new {
            client_id = config.Auth.Github.ClientId,
            client_secret = config.Auth.Github.ClientSecret,
            code,
            state
        }));

        Console.WriteLine("Requesting user data from Github");
        Console.WriteLine(request.ToString());
        using var response = await client.SendAsync(request);
        using var responseStream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<GithubAccessToken>(responseStream);
    }

    public async Task<GithubUserData?> GetUserData(string accessToken) {
        var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("codeduel.it", "1.0"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Console.WriteLine("Requesting user data from Github");
        Console.WriteLine(request.ToString());

        using HttpResponseMessage response = await client.SendAsync(request);

        using var responseStream = await response.Content.ReadAsStreamAsync();
        Console.WriteLine("Github user data:");
        Console.WriteLine(await new StreamReader(responseStream).ReadToEndAsync());
        return await JsonSerializer.DeserializeAsync<GithubUserData>(responseStream);
    }

    public async Task<List<GithubEmail>?> GetUserEmails(string accessToken) {
        var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using HttpResponseMessage response = await client.SendAsync(request);

        using var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<GithubEmail>>(responseStream);
    }

    public async Task<string?> GetUserPrimaryEmail(string accessToken) {
        var emails = await GetUserEmails(accessToken);
        if (emails == null) return null;

        var primaryEmail = emails.Find(static e => e.Verified && e.Primary);

        return primaryEmail?.Email;
    }
}
