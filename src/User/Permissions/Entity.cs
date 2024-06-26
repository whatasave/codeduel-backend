using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Permissions;

[Table("permissions")]
public class Entity {
    [Key]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public required User.Entity User { get; set; }
    public int CompactNotation { get; set; } = 0;
    public bool CanEditOwnChallenges { get; set; } = false;
    public bool CanEditChallenges { get; set; } = false;
    public bool CanEditUserPermissions { get; set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
