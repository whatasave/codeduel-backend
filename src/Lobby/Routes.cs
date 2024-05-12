namespace Lobby;

class Routes {
    public static void Setup(RouteGroupBuilder group) {
        group.MapGet("/lobby", Lobby.controller);
    }
}