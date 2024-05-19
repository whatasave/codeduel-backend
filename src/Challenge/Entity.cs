using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge;

[Table("challenge")]
public class Entity {
    [Key]
    public required int Id { get; set; }
    public required User.Entity Owner { get; set; }
    [MaxLength(50)]
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Content { get; set; }
    [DefaultValue("CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    [DefaultValue("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
