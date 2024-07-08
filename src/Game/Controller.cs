using Auth;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Game;
public class Controller(Service service) {
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

    [InternalAuth]
    public void Create(CreateGame request) {
        service.CreateGame(request);
    }

    public Results<Ok<GameWithUsersData>, NotFound> FindById(string uniqueId) {
        var results = service.GetGameResults(uniqueId);
        if (results == null) return TypedResults.NotFound();
        return TypedResults.Ok(results);
    }

    [InternalAuth]
    public NoContent Submit(string uniqueId, UpdateSubmission request) {
        service.UpdateSubmission(uniqueId, request);
        return TypedResults.NoContent();
    }

    [InternalAuth]
    public NoContent EndGame(string uniqueId) {
        service.EndGame(uniqueId);
        return TypedResults.NoContent();
    }

    [Auth]
    public NoContent ShareCode(HttpContext context, ShareCodeRequest request) {
        var auth = context.Auth();
        service.ShareGameCode(auth.UserId, request);
        return TypedResults.NoContent();
    }

    public Results<Ok<GameWithUsersData>, NotFound> GameResults(string uniqueId) {
        var results = service.GetGameResults(uniqueId);
        if (results == null) return TypedResults.NotFound();
        return TypedResults.Ok(results);
    }

    public IEnumerable<GameWithUserData> GetUserGames(int userId) {
        return service.GetGamesByUserId(userId);
    }

}
