using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth;

[Table("auth")]
public class Entity {
    [Key]
    public required int Id { get; set; }
    public required User.Entity User { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
