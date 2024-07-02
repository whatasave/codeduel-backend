using Auth;
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
    public ActionResult Create(CreateGame request) {
        var result = service.CreateGame(request);
        if (result == null) return new BadRequestResult();
        return new CreatedAtActionResult("FindById", "Controller", new { uniqueId = result.UniqueId }, result);
    }

    public GameWithUsersData FindById(string uniqueId) {
        return service.GetGameResults(uniqueId);
    }

    [ServiceFilter(typeof(InternalAuthFilter))]
    public ActionResult Submit(string uniqueId, UpdateSubmission request) {
        var success = service.SubmitGameAction(uniqueId, request);
        if (!success) {
            return new BadRequestResult();
        }
        return new OkResult();
    }

    [ServiceFilter(typeof(InternalAuthFilter))]
    public ActionResult EndGame(string uniqueId) {
        var success = service.EndGame(uniqueId);
        if (!success) return new BadRequestResult();
        return new OkResult();
    }

    [ServiceFilter(typeof(AuthFilter))]
    public ActionResult ShareCode(HttpContext context, ShareCodeRequest request) {
        var auth = context.Auth();
        var success = service.ShareGameCode(auth.UserId, request);
        if (!success) return new BadRequestResult();
        return new OkResult();
    }

    public GameWithUsersData GameResults(string uniqueId) {
        return service.GetGameResults(uniqueId);
    }

    public IEnumerable<GameWithUserData> GetUserGames(int userId) {
        return service.GetGamesByUserId(userId);
    }

}
