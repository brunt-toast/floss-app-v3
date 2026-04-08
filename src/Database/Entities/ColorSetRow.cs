using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("ColorSetRows")]
public class ColorSetRow
{
    [Key] [Column(TypeName = "TEXT")] public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required]
    [Column(TypeName = "TEXT")]
    [ForeignKey(nameof(ColorSet))]
    public Guid ColorSetId { get; set; }

    [Required] public ColorSet ColorSet { get; set; } = null!;

    [Required] [StringLength(255)] public string Name { get; set; } = string.Empty;

    [Required] [StringLength(100)] public string Floss { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "INTEGER")]
    public byte Red { get; set; }

    [Required]
    [Column(TypeName = "INTEGER")]
    public byte Green { get; set; }

    [Required]
    [Column(TypeName = "INTEGER")]
    public byte Blue { get; set; }
}

