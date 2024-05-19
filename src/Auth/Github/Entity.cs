using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Github;

[Table("auth_github")]
public class Entity {
    [Key]
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required string Provider { get; set; }
    public required int ProviderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
