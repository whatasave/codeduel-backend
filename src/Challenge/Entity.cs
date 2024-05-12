namespace Challenge;
public class Routes {
    private Service service;

    public Routes(Service service) {
        this.service = service;
    }

    public Routes(DatabaseContext database) : this(new Service(database)) {
    }
    
    public void Setup(RouteGroupBuilder group) {
        group.MapGet("/{id}", findById);
    }

    public Challenge findById(int id) {
        return service.findById(id);
    }
}
