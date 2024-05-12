namespace Lobby;
public class Controller(Service service) {
    public Controller(DatabaseContext database) : this(new Service(database)) {
    }
    
    public void Setup(RouteGroupBuilder group) {
        group.MapGet("/{id}", findById);
    }

    public Entity findById(int id) {
        return service.findById(id);
    }
}
