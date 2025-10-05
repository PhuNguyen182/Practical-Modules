namespace PracticalUtilities.StringUtils
{
    public static class StringExtension
    {
        public static bool FastEqual(this string s, string target) => string.CompareOrdinal(s, target) == 0;
    }
}