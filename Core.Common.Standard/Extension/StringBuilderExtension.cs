using System.Text;

namespace KY.Core.Extension
{
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
    }
}