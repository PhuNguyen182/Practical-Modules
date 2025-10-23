namespace PracticalModules.Localization.Components
{
    public class LocalizationUtils
    {
        private string FixHindiSlash(string inputText)
        {
            return inputText.Contains("/") ? UnicodeToKrutidev.ReplaceFirstOccurrence(inputText, "/", "@") : inputText;
        }

        private string FixHindiColon(string inputText)
        {
            return inputText.Contains(":") ? UnicodeToKrutidev.ReplaceFirstOccurrence(inputText, ":", "%") : inputText;
        }
    }
}
