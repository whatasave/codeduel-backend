using dotenv.net;

namespace Config;

public record Config(
    Database Database,
    Cors Cors,
    Cookie Cookie,
    Auth Auth
) {
    public static Config FromEnv() {
        DotEnv.Load();
        return new Config(
            Database.FromEnv(),
            Cors.FromEnv(),
            Cookie.FromEnv(),
            Auth.FromEnv()
        );
    }
}

public record Database(
    string Host,
    string Port,
    string DatabaseName,
    string User,
    string Password
) {
    public static Database FromEnv() {
        return new Database(
            Env.GetString("DB_HOST", "localhost"),
            Env.GetString("DB_PORT", "3306"),
            Env.GetString("DB_NAME", "codeduel"),
            Env.GetString("DB_USER", "codeduel"),
            Env.GetString("DB_PASSWORD", "codeduel")
        );
    }
    public string ConnectionString() {
        return $"server={Host};port={Port};database={DatabaseName};user={User};password={Password}";
    }
}

public record Cors(
    string Origins,
    string Methods,
    string Headers,
    bool AllowCredentials
) {
    public static Cors FromEnv() {
        return new Cors(
            Env.GetString("CORS_ORIGIN", "http://localhost:5173"),
            Env.GetString("CORS_METHODS", "GET, POST, PUT, DELETE, PATCH, OPTIONS"),
            Env.GetString("CORS_HEADERS", "Content-Type, x-token, Accept, Content-Length, Accept-Encoding, Authorization,X-CSRF-Token"),
            Env.GetBool("CORS_CREDENTIALS", true)
        );
    }
}

public record Cookie(
    bool HttpOnly,
    string Domain,
    string Path,
    bool Secure
) {
    public static Cookie FromEnv() {
        return new Cookie(
            Env.GetBool("COOKIE_HTTP_ONLY", true),
            Env.GetString("COOKIE_DOMAIN", "localhost"),
            Env.GetString("COOKIE_PATH", "/"),
            Env.GetBool("COOKIE_SECURE", true)
        );
    }
}

public record Auth(
    TimeSpan AccessTokenExpires,
    TimeSpan RefreshTokenExpires,
    string AccessTokenCookieName,
    string RefreshTokenCookieName,
    string Secret
) {
    public static Auth FromEnv() {
        return new Auth(
            Env.GetTimeSpan("ACCESS_TOKEN_EXPIRES", TimeSpan.FromMinutes(3)),
            Env.GetTimeSpan("REFRESH_TOKEN_EXPIRES", TimeSpan.FromDays(30)),
            Env.GetString("ACCESS_TOKEN_COOKIE_NAME", "access_token"),
            Env.GetString("REFRESH_TOKEN_COOKIE_NAME", "refresh_token"),
            Env.GetString("SECRET", "secret")
        );
    }
}

public static class Env {
    public static string GetString(string name, string defaultValue) {
        var env = Environment.GetEnvironmentVariable(name);
        if (env == null) {
            Console.WriteLine($"Environment variable {name} not found, using default value {defaultValue}");
            return defaultValue;
        }
        return env;
    }

    public static bool GetBool(string name, bool defaultValue) {
        var env = Environment.GetEnvironmentVariable(name);
        if (env == null) {
            Console.WriteLine($"Environment variable {name} not found, using default value {defaultValue}");
            return defaultValue;
        }
        return env == "true";
    }

    public static int GetInt(string name, int defaultValue) {
        var env = Environment.GetEnvironmentVariable(name);
        if (env == null) {
            Console.WriteLine($"Environment variable {name} not found, using default value {defaultValue}");
            return defaultValue;
        }
        return int.Parse(env);
    }

    public static TimeSpan GetTimeSpan(string name, TimeSpan defaultValue) {
        var env = Environment.GetEnvironmentVariable(name);
        if (env == null) {
            Console.WriteLine($"Environment variable {name} not found, using default value {defaultValue}");
            return defaultValue;
        }
        return TimeSpan.FromSeconds(int.Parse(env));
    }
}
