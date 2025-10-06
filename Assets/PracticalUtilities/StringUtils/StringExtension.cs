using System.Text;

namespace PracticalUtilities.StringUtils
{
    public static class StringExtension
    {
        public static bool FastEqual(this string s, string target) => string.CompareOrdinal(s, target) == 0;
        
        /// <summary>
        /// Converts a string to lower snake case format.
        /// Example: "HelloMyWorld" -> "hello_my_world", "Hello My World" -> "hello_my_world"
        /// </summary>
        /// <param name="input">The input string to convert</param>
        /// <returns>The string converted to lower snake case</returns>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            var result = new StringBuilder();
            var previousChar = char.MinValue;
            
            foreach (var currentChar in input)
            {
                // Skip spaces and convert to underscores
                if (char.IsWhiteSpace(currentChar))
                {
                    if (previousChar != '_' && result.Length > 0)
                    {
                        result.Append('_');
                    }
                    previousChar = '_';
                    continue;
                }
                
                // Add underscore before uppercase letters (except at the beginning)
                if (char.IsUpper(currentChar) && result.Length > 0 && previousChar != '_')
                {
                    result.Append('_');
                }
                
                // Convert to lowercase and add to result
                result.Append(char.ToLower(currentChar));
                previousChar = currentChar;
            }
            
            return result.ToString();
        }
    }
}