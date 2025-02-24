using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Auth;

// Serve una svolta per dare una svolta.
public class Service {
    private readonly Config.Config config;
    private readonly Repository repository;
    private readonly Permissions.Service permissions;
    private readonly JwtSecurityTokenHandler jwt;

    public Service(Config.Config config, Repository repository, Permissions.Service permissions) {
        this.config = config;
        this.repository = repository;
        this.permissions = permissions;
        jwt = new();
        jwt.InboundClaimTypeMap.Clear();
    }

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

        var userId = int.Parse(claims.Claims.First(c => c.Type.Equals("sub")).Value);
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
        if (config.Auth.Secret.Length < 32) throw new ArgumentException("Secret key must be at least 32 characters long.");
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

    public async Task<string> GenerateAccessToken(User.User user) {
        if (config.Auth.Secret.Length < 32) throw new ArgumentException("Secret key must be at least 32 characters long.");
        return jwt.WriteToken(new JwtSecurityToken(
            claims: [
                new("sub", user.Id.ToString(), ClaimValueTypes.Integer32),
                new("username", user.Username),
                new("perms", (await permissions.FindCompactByUserId(user.Id)).ToString())
            ],
            issuer: config.Auth.JwtIssuer,
            expires: DateTime.Now.Add(config.Auth.RefreshTokenExpires),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Auth.Secret)),
                SecurityAlgorithms.HmacSha256
            )
        ));
    }

    public async Task<TokenPair> GenerateTokens(User.User user) {
        var refreshToken = GenerateRefreshToken(user);
        var accessToken = await GenerateAccessToken(user);

        await SaveRefreshToken(user.Id, refreshToken);

        return new(accessToken, refreshToken);
    }

    public async Task SaveRefreshToken(int userId, string refreshToken) {
        await repository.SaveRefreshToken(userId, refreshToken);
    }

    public async Task RemoveRefreshToken(int userId) {
        await repository.RemoveRefreshToken(userId);
    }
}
