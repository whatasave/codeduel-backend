using System.Threading.Tasks;
using Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace User;
public class Controller(Service service) {
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
        group.MapGet("/", FindByUsername);
        group.MapGet("/profile", (Delegate)GetProfile);
        group.MapGet("/list", FindAll);
    }

    public async Task<Results<Ok<User>, NotFound>> FindById(int id) {
        Console.WriteLine($"--[User] Find By Id {id}--");
        var user = await service.FindById(id);
        if (user == null) return TypedResults.NotFound();
        return TypedResults.Ok(user);
    }

    public async Task<Results<Ok<User>, NotFound>> FindByUsername([FromQuery] string username) {
        Console.WriteLine($"--[User] Find By Username {username}--");
        var user = await service.FindByUsername(username);
        if (user == null) return TypedResults.NotFound();
        return TypedResults.Ok(user);
    }

    [Auth]
    public async Task<Results<Ok<User>, NotFound>> GetProfile(HttpContext context) {
        Console.WriteLine("--[User] Get Profile--");
        var auth = context.Auth();
        var user = await service.FindById(auth.UserId);
        if (user == null) return TypedResults.NotFound();
        return TypedResults.Ok(user);
    }

    public async Task<IEnumerable<UserListItem>> FindAll() {
        Console.WriteLine("--[User] Find All--");
        return await service.FindAll();
    }
}
