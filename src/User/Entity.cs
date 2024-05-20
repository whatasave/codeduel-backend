namespace User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user")]
public class Entity {
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Name { get; set; }
    public string? Avatar { get; set; }
    public string? BackgroundImage { get; set; }
    public string? Biography { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
