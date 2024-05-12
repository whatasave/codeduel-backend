namespace Challenge;

class Routes {
    public static void Setup(RouteGroupBuilder group) {
        group.MapGet("/challenge", Challenge.controller);
    }
}