using System.Threading.Tasks;
using Auth;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Challenge;

public class Controller(Service service) {
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", FindAll);
        group.MapGet("/{id}", FindById);
        group.MapPost("/", Create);
        group.MapPut("/{id}", Update);
        group.MapDelete("/{id}", Delete);
        group.MapGet("/random", FindRandom);
    }

    public async Task<IEnumerable<Challenge>> FindAll() {
        Console.WriteLine("--[Challenge] Find All--");
        return await service.FindAll();
    }

    public async Task<Results<Ok<Challenge>, NotFound>> FindById(int id) {
        Console.WriteLine($"--[Challenge] Find By Id {id}--");
        var challenge = await service.FindById(id);
        if (challenge == null) return TypedResults.NotFound();
        return TypedResults.Ok(challenge);
    }

    [Auth]
    public async Task<Results<Ok<Challenge>, ForbidHttpResult>> Create(HttpContext context, CreateChallenge challenge) {
        Console.WriteLine("--[Challenge] Create--");
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        return TypedResults.Ok(await service.Create(challenge, auth.UserId));
    }

    [Auth]
    public async Task<Results<NoContent, ForbidHttpResult, NotFound>> Update(HttpContext context, int id, CreateChallenge challenge) {
        Console.WriteLine($"--[Challenge] Update {id}--");
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        var existingChallenge = await service.FindById(id);
        if (existingChallenge == null) return TypedResults.NotFound();
        if (existingChallenge.Owner.Id != auth.UserId) return TypedResults.Forbid();

        return TypedResults.NoContent();
    }

    [Auth]
    public async Task<Results<NoContent, ForbidHttpResult, NotFound>> Delete(HttpContext context, int id) {
        Console.WriteLine($"--[Challenge] Delete {id}--");
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        var existingChallenge = await service.FindById(id);
        if (existingChallenge == null) return TypedResults.NotFound();
        if (existingChallenge.Owner.Id != auth.UserId) return TypedResults.Forbid();

        await service.Delete(id);
        return TypedResults.NoContent();
    }

    public async Task<ChallengeDetailed> FindRandom() {
        Console.WriteLine("--[Challenge] Find Random--");
        return await service.FindRandom();
    }
}
