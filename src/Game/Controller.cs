using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

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

    public ActionResult Create(CreateGame request) {
        var result = service.CreateGame(request);
        if (result == null) return new BadRequestResult();
        return new CreatedAtActionResult("FindById", "Controller", new { uniqueId = result.UniqueId }, result);
    }

    public GameWithUsersData FindById(string uniqueId) {
        return service.GetGameResults(uniqueId);
    }

    public ActionResult Submit(string uniqueId, UpdateSubmission request) {
        var success = service.SubmitGameAction(uniqueId, request);
        if (!success) {
            return new BadRequestResult();
        }
        return new OkResult();
    }

    public ActionResult EndGame(string uniqueId) {
        var success = service.EndGame(uniqueId);
        if (!success) return new BadRequestResult();
        return new OkResult();
    }

    public ActionResult ShareCode(ShareCodeRequest request) {
        var userId = 1; // TODO: get userId from token
        var success = service.ShareGameCode(userId, request);
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
