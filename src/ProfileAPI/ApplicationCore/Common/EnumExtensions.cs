using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ProfileAPI.ApplicationCore.Common;

public static class EnumExtensions
{
    public static string? GetEnumDisplayNameValueOrName<T>(this T value)
        where T : struct, Enum
        => value.GetEnumDisplayNameValueOrNull() ?? Enum.GetName(value);

    public static string? GetEnumDisplayNameValueOrNull<T>(this T value)
        where T : Enum
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attribute = fieldInfo!.GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name;
    }
}
