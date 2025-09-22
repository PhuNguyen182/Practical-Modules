using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Base class for theme managers
    /// </summary>
    public abstract class BaseThemeManager : MonoBehaviour, IThemeManager
    {
        [Header("Manager Settings")]
        [SerializeField] protected bool autoInitialize = true;
        [SerializeField] protected bool debugMode = false;
        
        protected List<IThemeComponent> registeredComponents = new List<IThemeComponent>();
        protected ITheme currentTheme;
        protected bool isActive = true;
        
        public abstract string Category { get; }
        public virtual bool IsActive => isActive;
        public virtual ITheme CurrentTheme => currentTheme;
        
        public event Action<ITheme> OnThemeApplied;
        public event Action<ITheme> OnThemeApplying;
        
        protected virtual void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        /// <summary>
        /// Initializes the theme manager
        /// </summary>
        protected virtual void Initialize()
        {
            if (debugMode)
                Debug.Log($"[{Category} Theme Manager] Initialized");
        }
        
        public virtual bool ApplyTheme(ITheme theme)
        {
            if (theme == null)
            {
                Debug.LogWarning($"[{Category} Theme Manager] Cannot apply null theme");
                return false;
            }
            
            if (!IsActive)
            {
                Debug.LogWarning($"[{Category} Theme Manager] Manager is not active");
                return false;
            }
            
            if (debugMode)
                Debug.Log($"[{Category} Theme Manager] Applying theme: {theme.ThemeName}");
            
            OnThemeApplying?.Invoke(theme);
            
            // Apply theme to all registered components
            int successCount = 0;
            foreach (var component in registeredComponents.ToList()) // Create copy to avoid modification during iteration
            {
                if (component != null && component.SupportsThemeType(theme.GetType()))
                {
                    try
                    {
                        component.ApplyTheme(theme);
                        successCount++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{Category} Theme Manager] Failed to apply theme to {component}: {e.Message}");
                    }
                }
            }
            
            currentTheme = theme;
            
            if (debugMode)
                Debug.Log($"[{Category} Theme Manager] Applied theme to {successCount}/{registeredComponents.Count} components");
            
            OnThemeApplied?.Invoke(theme);
            return true;
        }
        
        public virtual void RegisterComponent(IThemeComponent component)
        {
            if (component == null)
            {
                Debug.LogWarning($"[{Category} Theme Manager] Cannot register null component");
                return;
            }
            
            if (!registeredComponents.Contains(component))
            {
                registeredComponents.Add(component);
                
                // Apply current theme if one is active
                if (currentTheme != null)
                {
                    component.ApplyTheme(currentTheme);
                }
                
                if (debugMode)
                    Debug.Log($"[{Category} Theme Manager] Registered component: {component}");
            }
        }
        
        public virtual void UnregisterComponent(IThemeComponent component)
        {
            if (component != null && registeredComponents.Contains(component))
            {
                registeredComponents.Remove(component);
                
                if (debugMode)
                    Debug.Log($"[{Category} Theme Manager] Unregistered component: {component}");
            }
        }
        
        public virtual List<IThemeComponent> GetRegisteredComponents()
        {
            return new List<IThemeComponent>(registeredComponents);
        }
        
        public virtual void RefreshCurrentTheme()
        {
            if (currentTheme != null)
            {
                ApplyTheme(currentTheme);
            }
        }
        
        /// <summary>
        /// Sets the active state of this manager
        /// </summary>
        /// <param name="active">Whether the manager should be active</param>
        public virtual void SetActive(bool active)
        {
            isActive = active;
            
            if (debugMode)
                Debug.Log($"[{Category} Theme Manager] Set active: {active}");
        }
        
        /// <summary>
        /// Clears all registered components
        /// </summary>
        [ContextMenu("Clear Registered Components")]
        public virtual void ClearRegisteredComponents()
        {
            registeredComponents.Clear();
            
            if (debugMode)
                Debug.Log($"[{Category} Theme Manager] Cleared all registered components");
        }
        
        /// <summary>
        /// Gets statistics about registered components
        /// </summary>
        /// <returns>Component statistics</returns>
        public virtual ComponentStats GetComponentStats()
        {
            return new ComponentStats
            {
                totalRegistered = registeredComponents.Count,
                activeComponents = registeredComponents.Count(c => c != null),
                nullComponents = registeredComponents.Count(c => c == null)
            };
        }
        
        protected virtual void OnDestroy()
        {
            registeredComponents.Clear();
        }
        
        protected virtual void OnValidate()
        {
            // Remove null components during validation
            registeredComponents.RemoveAll(c => c == null);
        }
    }
    
    /// <summary>
    /// Statistics about registered components
    /// </summary>
    [Serializable]
    public class ComponentStats
    {
        public int totalRegistered;
        public int activeComponents;
        public int nullComponents;
    }
}
