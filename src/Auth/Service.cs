using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth;

public class Service(Config.Config config, Repository repository, Permissions.Service permissions) {
    private readonly JwtSecurityTokenHandler jwt = new();

    public Service(Config.Config config, Database.DatabaseContext database) : this(config, new Repository(database), new Permissions.Service(database)) { }

    public RefreshTokenPayload ValidateRefreshToken(string token) {
        var claims = jwt.ValidateToken(
            token,
            new TokenValidationParameters {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret))
            },
            out SecurityToken validatedToken
        );
        var userId = int.Parse(claims.Claims.First(c => c.Type == "sub").Value);
        return new(userId);
    }

    public AccessTokenPayload ValidateAccessToken(string token) {
        var claims = jwt.ValidateToken(
            token,
            new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = config.Auth.JwtIssuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret))
            },
            out SecurityToken validatedToken
        );
        var userId = int.Parse(claims.Claims.First(c => c.Type == "sub").Value);
        var username = claims.Claims.First(c => c.Type == "username").Value;
        var perms = int.Parse(claims.Claims.First(c => c.Type == "perms").Value);
        var permissions = new Permissions.UserPermissions(perms);
        return new(userId, username, permissions.CompactNotation, permissions.Permissions);
    }

    public string GenerateRefreshToken(User.User user) {
        if (config.Auth.Secret.Length < 32) throw new ArgumentException("Secret key must be at least 16 characters long.");
        return jwt.WriteToken(new JwtSecurityToken(
            claims: [
                new("sub", user.Id.ToString(), System.Security.Claims.ClaimValueTypes.Integer32)
            ],
            expires: DateTime.Now.Add(config.Auth.RefreshTokenExpires),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret)),
                SecurityAlgorithms.HmacSha256
            )
        ));
    }

    public string GenerateAccessToken(User.User user) {
        if (config.Auth.Secret.Length < 32) throw new ArgumentException("Secret key must be at least 16 characters long.");
        return jwt.WriteToken(new JwtSecurityToken(
            claims: [
                new("sub", user.Id.ToString(), System.Security.Claims.ClaimValueTypes.Integer32),
                new("username", user.Username),
                new("perms", permissions.FindCompactByUserId(user.Id).ToString())
            ],
            issuer: config.Auth.JwtIssuer,
            expires: DateTime.Now.Add(config.Auth.RefreshTokenExpires),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret)),
                SecurityAlgorithms.HmacSha256
            )
        ));
    }

    public TokenPair GenerateTokens(User.User user) {
        var refreshToken = GenerateRefreshToken(user);
        var accessToken = GenerateAccessToken(user);

        SaveRefreshToken(user.Id, refreshToken);

        return new(accessToken, refreshToken);
    }

    public void SaveRefreshToken(int userId, string refreshToken) {
        repository.SaveRefreshToken(userId, refreshToken);
    }

    public void RemoveRefreshToken(int userId) {
        repository.RemoveRefreshToken(userId);
    }
}
