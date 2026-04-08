using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("HiddenBuiltinColorSets")]
public class HiddenBuiltinColorSet
{
    [Key] [StringLength(100)] public string BuiltinKey { get; set; } = string.Empty;
}
