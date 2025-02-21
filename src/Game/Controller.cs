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

    public async Task<IEnumerable<GameWithUsersData>> GetAll() {
        Console.WriteLine("--[Game] Get All--");
        return await service.GetAllGames();
    }

    [InternalAuth]
    public async Task Create(CreateGame request) {
        Console.WriteLine("--[Game] Create--");
        await service.CreateGame(request);
    }

    public async Task<Results<Ok<GameWithUsersData>, NotFound>> FindById(string uniqueId) {
        Console.WriteLine($"--[Game] Find By Id {uniqueId}--");
        var results = await service.GetGameResults(uniqueId);
        if (results == null) return TypedResults.NotFound();
        return TypedResults.Ok(results);
    }

    [InternalAuth]
    public NoContent Submit(string uniqueId, UpdateSubmission request) {
        Console.WriteLine($"--[Game] Submit {uniqueId}--");
        service.UpdateSubmission(uniqueId, request);
        return TypedResults.NoContent();
    }

    [InternalAuth]
    public async Task<NoContent> EndGame(string uniqueId) {
        Console.WriteLine($"--[Game] End Game {uniqueId}--");
        await service.EndGame(uniqueId);
        return TypedResults.NoContent();
    }

    [Auth]
    public async Task<NoContent> ShareCode(HttpContext context, ShareCodeRequest request) {
        Console.WriteLine("--[Game] Share Code--");
        var auth = context.Auth();
        await service.ShareGameCode(auth.UserId, request);
        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<GameWithUsersData>, NotFound>> GameResults(string uniqueId) {
        Console.WriteLine($"--[Game] Game Results {uniqueId}--");
        var results = await service.GetGameResults(uniqueId);
        if (results == null) return TypedResults.NotFound();
        return TypedResults.Ok(results);
    }

    public async Task<IEnumerable<GameWithUserData>> GetUserGames(int userId) {
        Console.WriteLine($"--[Game] Get User Games {userId}--");
        return await service.GetGamesByUserId(userId);
    }

}
