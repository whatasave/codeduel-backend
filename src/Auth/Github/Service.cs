using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Auth.Github;

public class Service(Repository repository, Config.Config config, User.Service userService) {
    public const string PROVIDER = "github";

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
        try {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("codeduel.it", "1.0"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonSerializer.Serialize(new {
                client_id = config.Auth.Github.ClientId,
                client_secret = config.Auth.Github.ClientSecret,
                code,
                state
            }), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<GithubAccessToken>(responseStream, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
        }
        catch (Exception e) {
            Console.WriteLine("[ERROR] " + e.Message);
            return null;
        }
    }

    public async Task<GithubUserData?> GetUserData(string accessToken) {
        try {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("codeduel.it", "1.0"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await client.SendAsync(request);
            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<GithubUserData>(responseStream, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
        }
        catch (Exception e) {
            Console.WriteLine("[ERROR] " + e.Message);
            return null;
        }
    }
}
