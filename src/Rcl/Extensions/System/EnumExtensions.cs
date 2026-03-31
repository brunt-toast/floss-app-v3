using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Rcl.Extensions.System;

public static class EnumExtensions
{
    public static string ToDisplayName(this Enum source)
    {
        return source.GetDisplayAttribute()?.GetName() ?? source.ToString();
    }

    public static string GetDescription(this Enum source)
    {
        return source.GetDisplayAttribute()?.GetDescription() ?? string.Empty;
    }

    private static DisplayAttribute? GetDisplayAttribute(this Enum source)
    {
        string sourceString = source.ToString();
        return source.GetType()
            .GetMember(sourceString)
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>();
    }
}
