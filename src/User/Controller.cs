using Microsoft.AspNetCore.Mvc;

namespace User;
public class Controller(Service service) {
    public Controller(Database.DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{username}", FindByUsername);
    }

    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult FindByUsername(string username) {
        var user = service.FindByUsername(username);
        if (user == null) {
            return Results.NotFound();
        }
        return Results.Ok(user);
    }
}
