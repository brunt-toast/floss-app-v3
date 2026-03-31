using System.Drawing;

namespace App.Types; 

internal readonly record struct SetColor 
{
    public string Name { get; init; }
    public string Floss { get; init; }
    public Color Color { get; init; }
}