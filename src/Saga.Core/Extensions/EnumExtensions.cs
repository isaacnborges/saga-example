using System.ComponentModel;

namespace Saga.Core.Extensions;
public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute
            ? attribute.Description
            : string.Empty;
    }
}

