namespace User;
public class Controller(Service service) {
    public Controller(DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
    }

    public User FindById(int id) {
        return service.FindById(id);
    }
}
