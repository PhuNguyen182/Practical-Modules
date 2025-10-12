using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Foundations.DataFlow.MicroData;

namespace Foundations.DataFlow.Examples
{
    /// <summary>
    /// Example player progress data - tracks player's journey through the game
    /// This demonstrates how to create a proper IGameData implementation
    /// </summary>
    [Serializable]
    public class PlayerProgressData : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Basic Info")]
        public string playerName = "New Player";
        public int currentLevel = 1;
        public float experiencePoints = 0f;
        
        [Header("Game Progress")]
        public bool hasCompletedTutorial = false;
        public int highestLevelReached = 1;
        public float totalPlayTimeHours = 0f;
        
        [Header("Statistics")]
        public int enemiesDefeated = 0;
        public int itemsCollected = 0;
        public int deathCount = 0;
    }
    
    /// <summary>
    /// Example inventory data - tracks player's items and currency
    /// </summary>
    [Serializable]
    public class InventoryData : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Currency")]
        public int goldCoins = 100;
        public int premiumGems = 0;
        
        [Header("Items")]
        public bool hasHealthPotion = false;
        public bool hasManaPotion = false;
        public bool hasRareWeapon = false;
        
        [Header("Equipment")]
        public int weaponLevel = 1;
        public int armorLevel = 1;
        public Color playerColor = Color.white;
    }
    
    /// <summary>
    /// Example game settings - tracks player preferences
    /// </summary>
    [Serializable]
    public class GameSettings : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Audio")]
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public bool audioEnabled = true;
        
        [Header("Graphics")]
        public bool fullScreenMode = true;
        public int qualityLevel = 2;
        public bool vSyncEnabled = true;
        
        [Header("Controls")]
        public bool invertYAxis = false;
        public float mouseSensitivity = 1f;
        
        [Header("Gameplay")]
        public bool showTutorials = true;
        public bool enableAutoSave = true;
    }
    
    /// <summary>
    /// Example achievement data - tracks unlocked achievements
    /// </summary>
    [Serializable]
    public class AchievementData : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Basic Achievements")]
        public bool firstKill = false;
        public bool firstDeath = false;
        public bool firstLevelUp = false;
        
        [Header("Progress Achievements")]
        public bool reachedLevel10 = false;
        public bool reachedLevel25 = false;
        public bool reachedLevel50 = false;
        
        [Header("Collection Achievements")]
        public bool collected100Items = false;
        public bool collected500Items = false;
        public bool collected1000Items = false;
        
        [Header("Combat Achievements")]
        public bool defeated100Enemies = false;
        public bool defeatedBoss = false;
        public bool perfectCombat = false;
    }
    
    /// <summary>
    /// Example enum for demonstrating enum support in the tool
    /// </summary>
    public enum DifficultyLevel
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3
    }
    
    /// <summary>
    /// Example game configuration with enum support
    /// </summary>
    [Serializable]
    public class GameConfiguration : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Difficulty")]
        public DifficultyLevel difficulty = DifficultyLevel.Normal;
        public float difficultyMultiplier = 1f;
        
        [Header("World Settings")]
        public Vector3 spawnPosition = Vector3.zero;
        public Vector2 worldSize = new Vector2(100f, 100f);
        
        [Header("Debug")]
        public bool debugMode = false;
        public bool showFPS = false;
        public bool godMode = false;
    }
}
