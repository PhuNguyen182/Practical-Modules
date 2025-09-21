using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Materials
{
    /// <summary>
    /// Represents a complete material state for time-based material transitions
    /// </summary>
    [Serializable]
    public class MaterialState : IVisualState
    {
        [Header("State Configuration")]
        public string stateId;
        public TimeCondition timeCondition;
        
        [Header("Material Properties")]
        public MaterialColorProperty[] ColorProperties = new MaterialColorProperty[0];
        public MaterialFloatProperty[] FloatProperties = new MaterialFloatProperty[0];
        public MaterialVectorProperty[] VectorProperties = new MaterialVectorProperty[0];
        public MaterialTextureProperty[] TextureProperties = new MaterialTextureProperty[0];
        
        public string StateId => stateId;
        public TimeCondition TimeCondition => timeCondition;
        
        public MaterialState()
        {
            stateId = "Default";
            timeCondition = new TimeCondition();
        }
        
        public MaterialState(string stateId, TimeCondition timeCondition)
        {
            this.stateId = stateId;
            this.timeCondition = timeCondition;
        }
        
        /// <summary>
        /// Creates a copy of this material state
        /// </summary>
        /// <returns>Deep copy of the material state</returns>
        public MaterialState Clone()
        {
            var clone = new MaterialState(StateId, TimeCondition)
            {
                ColorProperties = new MaterialColorProperty[ColorProperties.Length],
                FloatProperties = new MaterialFloatProperty[FloatProperties.Length],
                VectorProperties = new MaterialVectorProperty[VectorProperties.Length],
                TextureProperties = new MaterialTextureProperty[TextureProperties.Length]
            };
            
            // Clone color properties
            for (int i = 0; i < ColorProperties.Length; i++)
            {
                clone.ColorProperties[i] = ColorProperties[i].Clone();
            }
            
            // Clone float properties
            for (int i = 0; i < FloatProperties.Length; i++)
            {
                clone.FloatProperties[i] = FloatProperties[i].Clone();
            }
            
            // Clone vector properties
            for (int i = 0; i < VectorProperties.Length; i++)
            {
                clone.VectorProperties[i] = VectorProperties[i].Clone();
            }
            
            // Clone texture properties
            for (int i = 0; i < TextureProperties.Length; i++)
            {
                clone.TextureProperties[i] = TextureProperties[i].Clone();
            }
            
            return clone;
        }
        
        /// <summary>
        /// Validates the material state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(StateId))
            {
                Debug.LogError("MaterialState: StateId cannot be null or empty");
                return false;
            }
            
            // Validate property arrays
            if (ColorProperties == null) ColorProperties = new MaterialColorProperty[0];
            if (FloatProperties == null) FloatProperties = new MaterialFloatProperty[0];
            if (VectorProperties == null) VectorProperties = new MaterialVectorProperty[0];
            if (TextureProperties == null) TextureProperties = new MaterialTextureProperty[0];
            
            return true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this material state
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            var condition = TimeCondition;
            var timeDesc = condition.Hour switch
            {
                -1 => "Any time",
                0 => "Midnight",
                6 => "Dawn",
                12 => "Noon",
                18 => "Dusk",
                23 => "Late night",
                _ => $"{condition.Hour:00}:00"
            };
            
            var seasonDesc = condition.UseSeason ? $" ({condition.Season})" : "";
            var monthDesc = condition.Month != -1 ? $" Month {condition.Month}" : "";
            
            var propCount = ColorProperties.Length + FloatProperties.Length + VectorProperties.Length + TextureProperties.Length;
            
            return $"{StateId}: {timeDesc}{seasonDesc}{monthDesc} ({propCount} properties)";
        }
        
        /// <summary>
        /// Adds a color property to this state
        /// </summary>
        /// <param name="propertyName">Name of the shader property</param>
        /// <param name="value">Color value</param>
        public void AddColorProperty(string propertyName, Color value)
        {
            var newProperties = new MaterialColorProperty[ColorProperties.Length + 1];
            ColorProperties.CopyTo(newProperties, 0);
            newProperties[ColorProperties.Length] = new MaterialColorProperty { PropertyName = propertyName, Value = value };
            ColorProperties = newProperties;
        }
        
        /// <summary>
        /// Adds a float property to this state
        /// </summary>
        /// <param name="propertyName">Name of the shader property</param>
        /// <param name="value">Float value</param>
        public void AddFloatProperty(string propertyName, float value)
        {
            var newProperties = new MaterialFloatProperty[FloatProperties.Length + 1];
            FloatProperties.CopyTo(newProperties, 0);
            newProperties[FloatProperties.Length] = new MaterialFloatProperty { PropertyName = propertyName, Value = value };
            FloatProperties = newProperties;
        }
        
        /// <summary>
        /// Adds a vector property to this state
        /// </summary>
        /// <param name="propertyName">Name of the shader property</param>
        /// <param name="value">Vector4 value</param>
        public void AddVectorProperty(string propertyName, Vector4 value)
        {
            var newProperties = new MaterialVectorProperty[VectorProperties.Length + 1];
            VectorProperties.CopyTo(newProperties, 0);
            newProperties[VectorProperties.Length] = new MaterialVectorProperty { PropertyName = propertyName, Value = value };
            VectorProperties = newProperties;
        }
        
        /// <summary>
        /// Adds a texture property to this state
        /// </summary>
        /// <param name="propertyName">Name of the shader property</param>
        /// <param name="value">Texture value</param>
        public void AddTextureProperty(string propertyName, Texture value)
        {
            var newProperties = new MaterialTextureProperty[TextureProperties.Length + 1];
            TextureProperties.CopyTo(newProperties, 0);
            newProperties[TextureProperties.Length] = new MaterialTextureProperty { PropertyName = propertyName, Value = value };
            TextureProperties = newProperties;
        }
        
        /// <summary>
        /// Removes a property by name
        /// </summary>
        /// <param name="propertyName">Name of the property to remove</param>
        public void RemoveProperty(string propertyName)
        {
            // Remove from color properties
            for (int i = 0; i < ColorProperties.Length; i++)
            {
                if (ColorProperties[i].PropertyName == propertyName)
                {
                    var newProperties = new MaterialColorProperty[ColorProperties.Length - 1];
                    Array.Copy(ColorProperties, 0, newProperties, 0, i);
                    Array.Copy(ColorProperties, i + 1, newProperties, i, ColorProperties.Length - i - 1);
                    ColorProperties = newProperties;
                    return;
                }
            }
            
            // Remove from float properties
            for (int i = 0; i < FloatProperties.Length; i++)
            {
                if (FloatProperties[i].PropertyName == propertyName)
                {
                    var newProperties = new MaterialFloatProperty[FloatProperties.Length - 1];
                    Array.Copy(FloatProperties, 0, newProperties, 0, i);
                    Array.Copy(FloatProperties, i + 1, newProperties, i, FloatProperties.Length - i - 1);
                    FloatProperties = newProperties;
                    return;
                }
            }
            
            // Remove from vector properties
            for (int i = 0; i < VectorProperties.Length; i++)
            {
                if (VectorProperties[i].PropertyName == propertyName)
                {
                    var newProperties = new MaterialVectorProperty[VectorProperties.Length - 1];
                    Array.Copy(VectorProperties, 0, newProperties, 0, i);
                    Array.Copy(VectorProperties, i + 1, newProperties, i, VectorProperties.Length - i - 1);
                    VectorProperties = newProperties;
                    return;
                }
            }
            
            // Remove from texture properties
            for (int i = 0; i < TextureProperties.Length; i++)
            {
                if (TextureProperties[i].PropertyName == propertyName)
                {
                    var newProperties = new MaterialTextureProperty[TextureProperties.Length - 1];
                    Array.Copy(TextureProperties, 0, newProperties, 0, i);
                    Array.Copy(TextureProperties, i + 1, newProperties, i, TextureProperties.Length - i - 1);
                    TextureProperties = newProperties;
                    return;
                }
            }
        }
    }
    
    /// <summary>
    /// Represents a material color property
    /// </summary>
    [Serializable]
    public class MaterialColorProperty
    {
        [Tooltip("Name of the shader property (e.g., _Color, _EmissionColor)")]
        public string PropertyName;
        
        [Tooltip("Color value for the property")]
        public Color Value = Color.white;
        
        public MaterialColorProperty()
        {
            PropertyName = "_Color";
            Value = Color.white;
        }
        
        public MaterialColorProperty(string propertyName, Color value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        
        public MaterialColorProperty Clone()
        {
            return new MaterialColorProperty(PropertyName, Value);
        }
    }
    
    /// <summary>
    /// Represents a material float property
    /// </summary>
    [Serializable]
    public class MaterialFloatProperty
    {
        [Tooltip("Name of the shader property (e.g., _Metallic, _Smoothness)")]
        public string PropertyName;
        
        [Tooltip("Float value for the property")]
        public float Value = 0f;
        
        public MaterialFloatProperty()
        {
            PropertyName = "_Metallic";
            Value = 0f;
        }
        
        public MaterialFloatProperty(string propertyName, float value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        
        public MaterialFloatProperty Clone()
        {
            return new MaterialFloatProperty(PropertyName, Value);
        }
    }
    
    /// <summary>
    /// Represents a material vector property
    /// </summary>
    [Serializable]
    public class MaterialVectorProperty
    {
        [Tooltip("Name of the shader property (e.g., _MainTex_ST)")]
        public string PropertyName;
        
        [Tooltip("Vector4 value for the property")]
        public Vector4 Value = Vector4.zero;
        
        public MaterialVectorProperty()
        {
            PropertyName = "_MainTex_ST";
            Value = new Vector4(1, 1, 0, 0);
        }
        
        public MaterialVectorProperty(string propertyName, Vector4 value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        
        public MaterialVectorProperty Clone()
        {
            return new MaterialVectorProperty(PropertyName, Value);
        }
    }
    
    /// <summary>
    /// Represents a material texture property
    /// </summary>
    [Serializable]
    public class MaterialTextureProperty
    {
        [Tooltip("Name of the shader property (e.g., _MainTex, _NormalMap)")]
        public string PropertyName;
        
        [Tooltip("Texture for the property")]
        public Texture Value;
        
        public MaterialTextureProperty()
        {
            PropertyName = "_MainTex";
            Value = null;
        }
        
        public MaterialTextureProperty(string propertyName, Texture value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        
        public MaterialTextureProperty Clone()
        {
            return new MaterialTextureProperty(PropertyName, Value);
        }
    }
}
