namespace Lobby;
public class Controller {
    private Service service;

    public Controller(Service service) {
        this.service = service;
    }

    public Controller(DatabaseContext database) : this(new Service(database)) {
    }
    
    public void Setup(RouteGroupBuilder group) {
        group.MapGet("/{id}", findById);
    }

    public Lobby findById(int id) {
        return service.findById(id);
    }
}
