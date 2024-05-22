namespace Game;
public class Controller(Service service) {
    public Controller(Database.DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
    }

    public GameWithUsersData FindById(string uniqueId) {
        return service.FindByUniqueId(uniqueId);
    }
}
