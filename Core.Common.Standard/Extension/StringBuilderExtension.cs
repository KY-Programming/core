using System.Text;

namespace KY.Core.Extension;

public static class StringBuilderExtension
{
    public static StringBuilder TrimEnd(this StringBuilder builder)
    {
        if (builder == null || builder.Length == 0)
        {
            return builder;
        }
        int index = builder.Length - 1;
        for (; index >= 0; index--)
        {
            if (!char.IsWhiteSpace(builder[index]))
            {
                break;
            }
        }
        if (index < builder.Length - 1)
        {
            builder.Length = index + 1;
        }
        return builder;
    }

    /// <summary>
    ///     Appends a value to the builder. When the string builder is not empty, the separator will added first
    /// </summary>
    public static StringBuilder AppendSeparated(this StringBuilder builder, string separator, string value)
    {
        if (builder.Length > 0)
        {
            builder.Append(separator);
        }
        builder.Append(value);
        return builder;
    }
}
