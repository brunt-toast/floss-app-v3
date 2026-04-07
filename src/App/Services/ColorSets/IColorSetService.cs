using App.Enums;
using App.Types;

namespace App.Services.ColorSets;

public interface IColorSetService
{
    IEnumerable<SetColor> GetColors(BuiltinColorSets set);

    IEnumerable<SetColor> GetColors(BuiltinColorSets set, Func<SetColor, bool> predicate);
}