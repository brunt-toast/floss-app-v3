using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("ColorSets")]
public class ColorSet
{
    [Key] [Column(TypeName = "TEXT")] public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required] [StringLength(255)] public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public bool IsHidden { get; set; }

    public ICollection<ColorSetRow> Colors { get; } = [];
}
