using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge;

[Table("challenge")]
public class Entity {
    [Key]
    public int Id { get; set; }
    [ForeignKey("Owner")]
    public required int OwnerId { get; set; }
    [MaxLength(50)]
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Content { get; set; }
    [DefaultValue("CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    [DefaultValue("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual User.Entity? Owner { get; set; }
    public virtual IEnumerable<TestCaseEntity>? TestCases { get; set; }
}

[Table("test_case")]
public class TestCaseEntity {
    [Key]
    public int Id { get; set; }
    public required int ChallengeId { get; set; }
    public required string Input { get; set; }
    public required string Output { get; set; }
    public required bool Hidden { get; set; }
}