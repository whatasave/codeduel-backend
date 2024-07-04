using Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace User;
public class Controller(Service service, AuthFilter authFilter) {
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
        group.MapGet("/", FindByUsername);
        group.MapGet("/profile", GetProfile).AddEndpointFilter(authFilter);
        group.MapGet("/list", FindAll);
    }

    public Results<Ok<User>, NotFound> FindById(int id) {
        var user = service.FindById(id);
        if (user == null) return TypedResults.NotFound();
        return TypedResults.Ok(user);
    }

    public Results<Ok<User>, NotFound> FindByUsername([FromQuery] string username) {
        var user = service.FindByUsername(username);
        if (user == null) return TypedResults.NotFound();
        return TypedResults.Ok(user);
    }

    public User GetProfile(HttpContext context) {
        Console.WriteLine("GetProfile");
        var auth = context.Auth();
        var user = service.FindById(auth.UserId)!;
        return user;
    }

    public IEnumerable<UserListItem> FindAll() {
        return service.FindAll();
    }
}
