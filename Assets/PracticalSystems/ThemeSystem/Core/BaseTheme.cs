using UnityEngine;
using System.Collections.Generic;
using System;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Base class for all theme implementations
    /// </summary>
    [Serializable]
    public abstract class BaseTheme : ScriptableObject, ITheme
    {
        [Header("Theme Information")]
        [SerializeField] protected string themeId = "";
        [SerializeField] protected string themeName = "";
        [SerializeField] protected string description = "";
        [SerializeField] protected string category = "";
        [SerializeField] protected int priority = 0;
        
        [Header("Theme Properties")]
        [SerializeField] protected List<ThemeProperty> properties = new List<ThemeProperty>();
        
        protected Dictionary<string, object> propertyCache = new Dictionary<string, object>();
        protected bool isActive = false;
        
        public virtual string ThemeId => themeId;
        public virtual string ThemeName => themeName;
        public virtual string Description => description;
        public virtual string Category => category;
        public virtual bool IsActive => isActive;
        public virtual int Priority => priority;
        
        protected virtual void OnEnable()
        {
            RefreshPropertyCache();
        }
        
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(themeId))
                themeId = System.Guid.NewGuid().ToString();
                
            RefreshPropertyCache();
        }
        
        /// <summary>
        /// Refreshes the internal property cache from the serialized properties
        /// </summary>
        protected virtual void RefreshPropertyCache()
        {
            propertyCache.Clear();
            foreach (var prop in properties)
            {
                if (!string.IsNullOrEmpty(prop.name) && prop.value != null)
                {
                    propertyCache[prop.name] = prop.value;
                }
            }
        }
        
        public virtual bool ApplyTo(IThemeComponent component)
        {
            if (component == null || !component.SupportsThemeType(GetType()))
                return false;
                
            component.ApplyTheme(this);
            return true;
        }
        
        public virtual Dictionary<string, object> GetProperties()
        {
            return new Dictionary<string, object>(propertyCache);
        }
        
        public virtual T GetProperty<T>(string propertyName, T defaultValue = default(T))
        {
            if (propertyCache.TryGetValue(propertyName, out var value))
            {
                if (value is T typedValue)
                    return typedValue;
                    
                // Try to convert the value
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    Debug.LogWarning($"Failed to convert property '{propertyName}' from {value?.GetType()} to {typeof(T)}");
                }
            }
            
            return defaultValue;
        }
        
        public virtual void SetProperty<T>(string propertyName, T value)
        {
            propertyCache[propertyName] = value;
            
            // Update serialized property if it exists
            var existingProp = properties.Find(p => p.name == propertyName);
            if (existingProp != null)
            {
                existingProp.value = value;
            }
            else
            {
                properties.Add(new ThemeProperty { name = propertyName, value = value });
            }
        }
        
        /// <summary>
        /// Marks this theme as active
        /// </summary>
        public virtual void SetActive(bool active)
        {
            isActive = active;
        }
        
        /// <summary>
        /// Creates a copy of this theme with a new ID
        /// </summary>
        public virtual ITheme Clone()
        {
            var clone = Instantiate(this);
            clone.themeId = System.Guid.NewGuid().ToString();
            clone.themeName = $"{themeName} (Copy)";
            clone.isActive = false;
            return clone;
        }
    }
    
    /// <summary>
    /// Serializable property for themes
    /// </summary>
    [Serializable]
    public class ThemeProperty
    {
        public string name;
        public object value;
    }
}
