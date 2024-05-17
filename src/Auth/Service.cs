using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth;

public class Service(Config.Config config, Repository repository, Permissions.Service permissions) {
    private JwtSecurityTokenHandler jwt = new();

    public Service(Config.Config config, Database.DatabaseContext database) : this(config, new Repository(database), new Permissions.Service(database)) { }

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
        return jwt.WriteToken(
            new JwtSecurityToken(
                claims: [
                    new("sub", user.Id.ToString())
                ],
                expires: DateTime.Now.Add(config.Auth.RefreshTokenExpires),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret)),
                    SecurityAlgorithms.HmacSha256
                )
            )
        );
    }

    public string GenerateAccessToken(User.User user) {
        return jwt.WriteToken(
            new JwtSecurityToken(
                claims: [
                    new("sub", user.Id.ToString()),
                    new("username", user.Username),
                    new("perms", permissions.FindCompactByUserId(user.Id).ToString())
                ],
                issuer: config.Auth.JwtIssuer,
                expires: DateTime.Now.Add(config.Auth.RefreshTokenExpires),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret)),
                    SecurityAlgorithms.HmacSha256
                )
            )
        );
    }

    public void SaveRefreshToken(int userId, string refreshToken) {
        repository.SaveRefreshToken(userId, refreshToken);
    }

    public void RemoveRefreshToken(int userId) {
        repository.RemoveRefreshToken(userId);
    }
}
