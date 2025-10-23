using System;
using System.Collections.Generic;
using I2.Loc;

namespace PracticalModules.Localization.Service
{
    public class I2LocalizationService : ILocalizationService
    {
        private bool _isDisposed;
        
        public event Action OnLanguageChanged;
        
        public I2LocalizationService()
        {
            LocalizationManager.OnLocalizeEvent += OnLanguageChange;
        }

        private void OnLanguageChange()
        {
            OnLanguageChanged?.Invoke();
        }
        
        public string GetTranslation(string key)
        {
            return LocalizationManager.GetTranslation(key);
        }

        public List<string> GetAllLanguages()
        {
            return LocalizationManager.GetAllLanguages();
        }

        public string GetCurrentLanguageCode(string language)
        {
            return LocalizationManager.GetLanguageCode(language);
        }

        public string GetLanguageName(string languageCode, bool exactMatch = true)
        {
            return LocalizationManager.GetLanguageFromCode(languageCode, exactMatch);
        }
        
        public string GetCurrentLanguage()
        {
            return LocalizationManager.CurrentLanguage;
        }
        
        public string GetLanguageCode(string language)
        {
            return LocalizationManager.GetLanguageCode(language);
        }
        
        public void SetCurrentLanguage(string language)
        {
            LocalizationManager.CurrentLanguage = language;
        }
        
        public void SetCurrentLanguageByCode(string languageCode)
        {
            LocalizationManager.CurrentLanguageCode = languageCode;
        }

        #region Disposing

        private void ReleaseUnmanagedResources()
        {
            LocalizationManager.OnLocalizeEvent -= OnLanguageChange;
            this.OnLanguageChanged = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._isDisposed)
                return;
            
            if (disposing)
            {
                ReleaseUnmanagedResources();
            }
            
            this._isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~I2LocalizationService()
        {
            Dispose(false);
        }
        
        #endregion
    }
}
