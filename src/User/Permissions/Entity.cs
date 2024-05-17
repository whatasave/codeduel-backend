using System.ComponentModel.DataAnnotations;

namespace Permissions;

public class Entity {
    [Key]
    public required User.Entity User { get; set; }
    public int CompactNotation { get; set; } = 0;
    public bool CanEditOwnChallenges { get; set; } = false;
    public bool CanEditChallenges { get; set; } = false;
    public bool CanEditUserPermissions { get; set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
