using System;
using UnityEngine;
using Newtonsoft.Json;
using Foundations.DataFlow.MicroData;

namespace Foundations.DataFlow.Examples
{
    /// <summary>
    /// Simple test script to generate sample PlayerPrefs data for testing the tool
    /// </summary>
    public class TestPlayerPrefsData : MonoBehaviour
    {
        [Header("Test Data Generation")]
        [SerializeField] private bool createDataOnStart = false;
        
        [Header("Sample Data")]
        [SerializeField] private string playerName = "TestPlayer";
        [SerializeField] private int playerLevel = 15;
        [SerializeField] private float experience = 2500f;
        
        private void Start()
        {
            if (this.createDataOnStart)
            {
                this.CreateSampleData();
            }
        }
        
        [ContextMenu("Create Sample PlayerPrefs Data")]
        public void CreateSampleData()
        {
            try
            {
                Debug.Log("🎮 Creating sample PlayerPrefs data for testing...");
                
                // Create sample PlayerProgressData
                var playerProgress = new PlayerProgressData
                {
                    playerName = this.playerName,
                    currentLevel = this.playerLevel,
                    experiencePoints = this.experience,
                    hasCompletedTutorial = true,
                    highestLevelReached = this.playerLevel,
                    totalPlayTimeHours = 12.5f,
                    enemiesDefeated = 85,
                    itemsCollected = 45,
                    deathCount = 2
                };
                
                // Create sample InventoryData
                var inventory = new InventoryData
                {
                    goldCoins = 750,
                    premiumGems = 15,
                    hasHealthPotion = true,
                    hasManaPotion = false,
                    hasRareWeapon = true,
                    weaponLevel = 4,
                    armorLevel = 3,
                    playerColor = Color.red
                };
                
                // Create sample GameSettings
                var settings = new GameSettings
                {
                    masterVolume = 0.9f,
                    musicVolume = 0.7f,
                    sfxVolume = 1.0f,
                    audioEnabled = true,
                    fullScreenMode = true,
                    qualityLevel = 2,
                    vSyncEnabled = true,
                    invertYAxis = false,
                    mouseSensitivity = 1.2f,
                    showTutorials = false,
                    enableAutoSave = true
                };
                
                // Save with different key patterns to test tool's detection
                this.SaveToPlayerPrefs("GameData_PlayerProgressData", playerProgress);
                this.SaveToPlayerPrefs("InventoryData", inventory);
                this.SaveToPlayerPrefs("Data_GameSettings", settings);
                
                Debug.Log("✅ Sample data created successfully!");
                Debug.Log("🔧 Now open Tools → Foundations → PlayerPrefs Data Manager to see the data!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to create sample data: {ex.Message}");
            }
        }
        
        [ContextMenu("Clear All Test Data")]
        public void ClearTestData()
        {
            try
            {
                var keys = new[] 
                {
                    "GameData_PlayerProgressData",
                    "InventoryData", 
                    "Data_GameSettings"
                };
                
                foreach (var key in keys)
                {
                    if (PlayerPrefs.HasKey(key))
                    {
                        PlayerPrefs.DeleteKey(key);
                        Debug.Log($"🗑️ Cleared: {key}");
                    }
                }
                
                PlayerPrefs.Save();
                Debug.Log("✅ All test data cleared!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to clear test data: {ex.Message}");
            }
        }
        
        [ContextMenu("Show Existing PlayerPrefs")]
        public void ShowExistingPlayerPrefs()
        {
            try
            {
                Debug.Log("🔍 Checking for existing PlayerPrefs data...");
                
                var testKeys = new[]
                {
                    "GameData_PlayerProgressData",
                    "PlayerProgressData",
                    "InventoryData",
                    "Data_InventoryData", 
                    "GameSettings",
                    "Data_GameSettings",
                    "Settings"
                };
                
                var foundCount = 0;
                foreach (var key in testKeys)
                {
                    if (PlayerPrefs.HasKey(key))
                    {
                        var value = PlayerPrefs.GetString(key);
                        Debug.Log($"📄 Found key: '{key}' ({value.Length} chars)");
                        
                        // Show preview
                        var preview = value.Length > 150 ? value.Substring(0, 150) + "..." : value;
                        Debug.Log($"   Content: {preview}");
                        foundCount++;
                    }
                }
                
                Debug.Log($"✅ Found {foundCount} PlayerPrefs keys with data");
                
                if (foundCount == 0)
                {
                    Debug.Log("⚠️ No data found. Create some test data first!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to check PlayerPrefs: {ex.Message}");
            }
        }
        
        private void SaveToPlayerPrefs<T>(string key, T data) where T : IGameData
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include
                };
                
                var json = JsonConvert.SerializeObject(data, settings);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                
                Debug.Log($"💾 Saved {typeof(T).Name} to '{key}' ({json.Length} chars)");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to save {typeof(T).Name}: {ex.Message}");
            }
        }
    }
}
