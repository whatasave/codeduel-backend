namespace AuthGithub;

public class Controller(Service service) {
    public Controller(DatabaseContext database) : this(new Service(database)) {}
    
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", Login);
        group.MapGet("/callback", Callback);
    }

    public void Login(int id) {
        var resp = new HttpResponseMessage();

        // var cookie = new CookieHeaderValue("session-id", "12345");
        // cookie.Expires = DateTimeOffset.Now.AddDays(1);
        // cookie.Domain = Request.RequestUri.Host;
        // cookie.Path = "/";

        // resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
        // return resp;

        // return service.findById(id);
    }
    
    public void Callback(int id) {
        return;
    }
}
