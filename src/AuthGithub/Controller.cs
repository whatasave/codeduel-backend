namespace AuthGithub;
public class Controller {
    private Service service;

    public Controller(Service service) {
        this.service = service;
    }

    public Controller(DatabaseContext database) : this(new Service(database)) {}
    
    public void Setup(RouteGroupBuilder group) {
        group.MapGet("/", Login);
        group.MapGet("/callback", Callback);
    }

    public Entity Login(int id) {
        return service.findById(id);
    }
    public Entity Callback(int id) {
        return service.findById(id);
    }
}
