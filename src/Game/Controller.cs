using Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Game;
public class Controller(Service service) {
    public Controller(Database.DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", GetAll);
        group.MapPost("/", Create);
        group.MapGet("/{uniqueId}", FindById);
        group.MapPatch("/{uniqueId}/submit", Submit);
        group.MapPatch("/{uniqueId}/endgame", EndGame);
        group.MapGet("/{uniqueId}/results", GameResults);
        group.MapGet("/user/{userId}", GetUserGames);
        group.MapPatch("/sharecode", ShareCode);
    }

    public IEnumerable<GameWithUsersData> GetAll() {
        return service.GetAllGames();
    }

    [ServiceFilter(typeof(InternalAuthFilter))]
    public Created<Game> Create(CreateGame request) {
        var result = service.CreateGame(request);
        return TypedResults.Created((Uri?)null, result);
    }

    public GameWithUsersData FindById(string uniqueId) {
        return service.GetGameResults(uniqueId);
    }

    [ServiceFilter(typeof(InternalAuthFilter))]
    public NoContent Submit(string uniqueId, UpdateSubmission request) {
        service.UpdateSubmission(uniqueId, request);
        return TypedResults.NoContent();
    }

    [ServiceFilter(typeof(InternalAuthFilter))]
    public NoContent EndGame(string uniqueId) {
        service.EndGame(uniqueId);
        return TypedResults.NoContent();
    }

    [ServiceFilter(typeof(AuthFilter))]
    public NoContent ShareCode(HttpContext context, ShareCodeRequest request) {
        var auth = context.Auth();
        service.ShareGameCode(auth.UserId, request);
        return TypedResults.NoContent();
    }

    public GameWithUsersData GameResults(string uniqueId) {
        return service.GetGameResults(uniqueId);
    }

    public IEnumerable<GameWithUserData> GetUserGames(int userId) {
        return service.GetGamesByUserId(userId);
    }

}
