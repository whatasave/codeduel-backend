
public class Controller {
    private readonly Auth.Repository authRepository;
    private readonly User.Repository userRepository;
    private readonly Game.Repository gameRepository;
    private readonly Challenge.Repository challengeRepository;
    private readonly Permissions.Repository permissionsRepository;

    private readonly User.Service userService;
    private readonly Game.Service gameService;
    private readonly Challenge.Service challengeService;
    private readonly Auth.Github.Service githubAuthService;
    private readonly Auth.Service authService;
    private readonly Permissions.Service permissionsService;

    private readonly User.Controller userController;
    private readonly Game.Controller gameController;
    private readonly Challenge.Controller challengeController;
    private readonly Auth.Github.Controller githubAuthController;
    private readonly Auth.Controller authController;
    private readonly Permissions.Controller permissionsController;

    public Controller(Config.Config config, Database.DatabaseContext database) {
        authRepository = new Auth.Repository(database);
        userRepository = new User.Repository(database);
        gameRepository = new Game.Repository(database);
        challengeRepository = new Challenge.Repository(database);
        permissionsRepository = new Permissions.Repository(database);

        userService = new User.Service(userRepository);
        gameService = new Game.Service(gameRepository);
        challengeService = new Challenge.Service(challengeRepository);
        permissionsService = new Permissions.Service(permissionsRepository);
        githubAuthService = new Auth.Github.Service(authRepository, config, userService);
        authService = new Auth.Service(config, authRepository, permissionsService);

        userController = new User.Controller(userService);
        gameController = new Game.Controller(gameService);
        challengeController = new Challenge.Controller(challengeService);
        githubAuthController = new Auth.Github.Controller(config, githubAuthService, authService);
        authController = new Auth.Controller(config, authService, userService);
        permissionsController = new Permissions.Controller(permissionsService);
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        userController.SetupRoutes(group.MapGroup("/user"));
        gameController.SetupRoutes(group.MapGroup("/game"));
        challengeController.SetupRoutes(group.MapGroup("/challenge"));
        githubAuthController.SetupRoutes(group.MapGroup("/auth/github"));
        authController.SetupRoutes(group.MapGroup("/auth"));
        permissionsController.SetupRoutes(group.MapGroup("/user/permissions"));
    }

    public void SetupMiddleware(WebApplication app) {
        var config = app.Services.GetRequiredService<Config.Config>();
        var authMiddleware = new Auth.Middleware(config, authService);
        app.Use(authMiddleware.Auth);
        app.Use(authMiddleware.OptionalAuth);
        app.Use(authMiddleware.InternalAuth);
    }
}