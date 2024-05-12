namespace Jwt;

public class Service(Repository repository) {

    public Service(DatabaseContext database) : this(
        new Repository(database)
    ) {}
    public string CreateJwt(int userId) {
        return "token";
    }
    public object ValidateJwt(string token) {
        if (token != "token") {
            throw new Exception("Invalid token");
        }

        // var jwt = ParseJwt(token);
        // if (jwt.ExpireAt < DateTime.Now) {
        //     throw new Exception("Token expired");
        // }

        // return jwt;

        return new JwtPayload {
            Sub = 1,
            ExpireAt = DateTime.Now.AddHours(1),
        };
    }
    public object ParseJwt(string Token) {
        return new { 
            Sub = 1,
            ExpireAt = DateTime.Now.AddHours(1),
        };
    }

    public string GenerateRefreshToken(User.User user) {
        return CreateJwt(user.Id);
    }
    public string GenerateAccessToken(User.User user) {
        return CreateJwt(user.Id);
    }

    public void SaveRefreshToken(int userId, string refreshToken) {
        repository.SaveRefreshToken(userId, refreshToken);
    }
    public void RemoveRefreshToken(int userId) {
        repository.RemoveRefreshToken(userId);
    }
}
