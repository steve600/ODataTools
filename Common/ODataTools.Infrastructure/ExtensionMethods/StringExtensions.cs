using System;

namespace ODataTools.Infrastructure.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert first character to lower.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
            {
                return str;
            }

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
        
        /// <summary>
        /// Convert first character to upper.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstCharacterToUpper(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
            {
                return str;
            }

            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }
    }
}
