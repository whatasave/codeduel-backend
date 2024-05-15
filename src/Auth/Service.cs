namespace Auth;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(
        new Repository(database)
    ) { }

    public string CreateJwt(int userId) {
        return "token";
    }

    public RefreshTokenPayload ValidateRefreshToken(string token) {
        // var jwt = ParseJwt(token);
        // if (jwt.ExpireAt < DateTime.Now) {
        //     throw new Exception("Token expired");
        // }

        // return jwt;

        return new(1, DateTime.Now.AddHours(1));
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
