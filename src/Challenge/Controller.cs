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

    public IEnumerable<Challenge> FindAll() {
        return service.FindAll();
    }

    public Results<Ok<Challenge>, NotFound> FindById(int id) {
        var challenge = service.FindById(id);
        if (challenge == null) return TypedResults.NotFound();
        return TypedResults.Ok(challenge);
    }

    [Auth]
    public Results<Ok<Challenge>, ForbidHttpResult> Create(HttpContext context, CreateChallenge challenge) {
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        return TypedResults.Ok(service.Create(challenge, auth.UserId));
    }

    [Auth]
    public Results<NoContent, ForbidHttpResult, NotFound> Update(HttpContext context, int id, CreateChallenge challenge) {
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        var existingChallenge = service.FindById(id);
        if (existingChallenge == null) return TypedResults.NotFound();
        if (existingChallenge.Owner.Id != auth.UserId) return TypedResults.Forbid();

        return TypedResults.NoContent();
    }

    [Auth]
    public Results<NoContent, ForbidHttpResult, NotFound> Delete(HttpContext context, int id) {
        var auth = context.Auth();
        if (!auth.Permissions.CanEditOwnChallenges) return TypedResults.Forbid();

        var existingChallenge = service.FindById(id);
        if (existingChallenge == null) return TypedResults.NotFound();
        if (existingChallenge.Owner.Id != auth.UserId) return TypedResults.Forbid();

        service.Delete(id);
        return TypedResults.NoContent();
    }

    public ChallengeDetailed FindRandom() {
        return service.FindRandom();
    }
}
