using App.Enums;
using App.Types;

namespace App.Services.ColorSets;

internal interface IColorSetService
{
    IEnumerable<SetColor> GetColors(BuiltinColorSets set);

    IEnumerable<SetColor> GetColors(BuiltinColorSets set, Func<SetColor, bool> predicate);
}