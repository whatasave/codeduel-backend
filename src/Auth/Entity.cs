using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Auth;
[Table("auth")]
[PrimaryKey(nameof(UserId), nameof(Provider))]
[Index(nameof(UserId), nameof(ProviderId), IsUnique = true, IsDescending = [true, false])]
public class Entity {
    public required int UserId { get; set; }
    public required string Provider { get; set; }
    public required int ProviderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

[Table("auth_refresh_token")]
public class RefreshTokenEntity {
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public required int UserId { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual User.Entity? User { get; set; }
}
