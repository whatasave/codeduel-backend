using Microsoft.Net.Http.Headers;

namespace AuthGithub;

public class Controller(Service service) {
    public Controller(DatabaseContext database) : this(new Service(database)) {}
    
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", Login);
        group.MapGet("/callback", Callback);
    }

    public void Login() {
        var resp = new HttpResponseMessage();

        Console.WriteLine("response: " + resp);

        var cookie = new CookieHeaderValue("session-id", "12345");
        // cookie.Expires = DateTimeOffset.Now.AddDays(1);
        // cookie.Domain = Request.RequestUri.Host;
        // cookie.Path = "/";
        Console.WriteLine("cookie: " + cookie);

        resp.Headers.Add("set-cookie", cookie.ToString());

        // return service.findById(id);
    }
    
    public void Callback(int id) {
        return;
    }
}
