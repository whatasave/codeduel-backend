using Microsoft.AspNetCore.Mvc;

namespace Challenge;
public class Controller(Service service, Auth.Service authService) {
    public Controller(Config.Config config, Database.DatabaseContext database) : this(
        new Service(database),
        new Auth.Service(config, database)
    ) {}

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", FindAll);
        group.MapGet("/{id}", FindById);
        group.MapPost("/", Create);
        group.MapPut("/{id}", Update);
        group.MapDelete("/{id}", Delete);

        group.MapGet("/random", FindRandom);
    }

    public IEnumerable<Challenge> FindAll() {
        return service.FindAll();
    }

    public Challenge FindById(int id) {
        return service.FindById(id);
    }

    public Challenge Create(HttpRequest request, CreateChallenge newChallenge) {
        // TODO add a middleware to get the user from the request
        var accessToken = request.Cookies["access_token"] ?? throw new Exception("Access token is required");
        var user = authService.ValidateAccessToken(accessToken) ?? throw new Exception("Invalid access token");

        // check permissions
        if (!user.Permissions.CanEditOwnChallenges) throw new Exception("User does not have permission to create challenges");

        var challenge = new Challenge(
            Id: 0,
            new User.UserListItem(user),
            newChallenge.Title,
            newChallenge.Description,
            newChallenge.Content
        );

        return service.Create(challenge);

    }

    public Challenge Update(HttpRequest request, int id, CreateChallenge challenge) {
        // TODO add a middleware to get the user from the request
        var accessToken = request.Cookies["access_token"] ?? throw new Exception("Access token is required");
        var user = authService.ValidateAccessToken(accessToken) ?? throw new Exception("Invalid access token");

        // check permissions
        if (!user.Permissions.CanEditOwnChallenges) throw new Exception("User does not have permission to edit challenges");

        var existingChallenge = service.FindById(id) ?? throw new Exception("Challenge not found");
        if (existingChallenge.Owner.Id != user.UserId) throw new Exception("User does not have permission to edit this challenge");

        var updatedChallenge = new Challenge(
            Id: id,
            new User.UserListItem(user),
            challenge.Title,
            challenge.Description,
            challenge.Content
        );

        return service.Update(updatedChallenge);
    }

    public void Delete(HttpRequest request, int id) {
        // TODO add a middleware to get the user from the request
        var accessToken = request.Cookies["access_token"] ?? throw new Exception("Access token is required");
        var user = authService.ValidateAccessToken(accessToken) ?? throw new Exception("Invalid access token");

        // check permissions
        if (!user.Permissions.CanEditOwnChallenges) throw new Exception("User does not have permission to delete challenges");

        var existingChallenge = service.FindById(id) ?? throw new Exception("Challenge not found");
        if (existingChallenge.Owner.Id != user.UserId) throw new Exception("User does not have permission to delete this challenge");

        service.Delete(id);
    }

    public Challenge FindRandom() {
        return service.FindRandom();
    }
}
