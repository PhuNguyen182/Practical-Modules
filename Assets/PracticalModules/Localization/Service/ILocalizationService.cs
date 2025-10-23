using System;
using System.Collections.Generic;

namespace PracticalModules.Localization.Service
{
    public interface ILocalizationService : IDisposable
    {
        public string GetTranslation(string key);
        public List<string> GetAllLanguages();
        public string GetCurrentLanguageCode(string language);
        public string GetLanguageName(string languageCode, bool exactMatch = true);
        public string GetCurrentLanguage();
        public string GetLanguageCode(string language);
        public void SetCurrentLanguage(string language);
        public void SetCurrentLanguageByCode(string languageCode);
    }
}
