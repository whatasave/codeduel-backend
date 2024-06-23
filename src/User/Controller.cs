using Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace User;
public class Controller(Service service) {
    public Controller(Database.DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
        group.MapGet("/", FindByUsername);
        group.MapGet("/profile", GetProfile);
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

    [ServiceFilter(typeof(AuthFilter))]
    public Ok<User> GetProfile(HttpContext context) {
        Console.WriteLine("entering /user/profile");
        var auth = context.Auth();
        var user = service.FindById(auth.UserId);
        return TypedResults.Ok(user);
    }
}
