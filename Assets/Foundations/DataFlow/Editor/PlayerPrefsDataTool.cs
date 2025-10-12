using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Foundations.DataFlow.MicroData;
using Newtonsoft.Json;

namespace Foundations.DataFlow.Editor
{
    /// <summary>
    /// PlayerPrefs Data Manager Tool
    /// A powerful Unity Editor tool for managing game data stored in PlayerPrefs
    /// Supports all [Serializable] classes that implement IGameData interface
    /// </summary>
    public class PlayerPrefsDataTool : EditorWindow
    {
        private VisualElement root;
        private ScrollView dataScrollView;
        private VisualElement dataContainer;
        private VisualElement emptyState;
        private Button loadAllButton;
        private Button saveAllButton;
        private Button deleteAllButton;
        private Label dataCountLabel;
        private Label lastActionLabel;
        
        private readonly List<PlayerPrefsDataEntry> dataEntries = new();
        private readonly Dictionary<Type, PlayerPrefsDataEntry> entryMap = new();
        
        [MenuItem("Tools/Foundations/PlayerPrefs Data Manager", false, 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<PlayerPrefsDataTool>();
            window.titleContent = new GUIContent("🎮 PlayerPrefs Data Manager");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        public void CreateGUI()
        {
            // Load UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Foundations/DataFlow/Editor/PlayerPrefsDataTool.uxml");
            if (visualTree == null)
            {
                Debug.LogError("❌ Could not load PlayerPrefsDataTool.uxml");
                return;
            }
            
            this.root = visualTree.CloneTree();
            this.rootVisualElement.Add(this.root);
            
            // Cache UI elements
            this.CacheUIElements();
            
            // Bind events
            this.BindEvents();
            
            // Initialize UI
            this.UpdateUI();
            
            Debug.Log("✅ PlayerPrefs Data Manager initialized successfully");
        }
        
        /// <summary>
        /// Caches references to UI elements for better performance
        /// </summary>
        private void CacheUIElements()
        {
            this.dataScrollView = this.root.Q<ScrollView>("data-scroll-view");
            this.dataContainer = this.root.Q<VisualElement>("data-container");
            this.emptyState = this.root.Q<VisualElement>("empty-state");
            this.loadAllButton = this.root.Q<Button>("load-all-button");
            this.saveAllButton = this.root.Q<Button>("save-all-button");
            this.deleteAllButton = this.root.Q<Button>("delete-all-button");
            this.dataCountLabel = this.root.Q<Label>("data-count-label");
            this.lastActionLabel = this.root.Q<Label>("last-action-label");
        }
        
        /// <summary>
        /// Binds event handlers to UI elements
        /// </summary>
        private void BindEvents()
        {
            this.loadAllButton?.RegisterCallback<ClickEvent>(_ => this.LoadAllData());
            this.saveAllButton?.RegisterCallback<ClickEvent>(_ => this.SaveAllData());
            this.deleteAllButton?.RegisterCallback<ClickEvent>(_ => this.ShowDeleteAllConfirmation());
            
            // Auto-scan for data types on first load
            EditorApplication.delayCall += this.ScanForGameDataTypes;
        }
        
        /// <summary>
        /// Scans PlayerPrefs keys and matches them with IGameData types
        /// </summary>
        private void ScanForGameDataTypes()
        {
            try
            {
                this.dataEntries.Clear();
                this.entryMap.Clear();
                
                // First, get all available IGameData types from assemblies
                var availableGameDataTypes = this.GetAvailableGameDataTypes();
                Debug.Log($"🔍 Found {availableGameDataTypes.Count} available IGameData types: {string.Join(", ", availableGameDataTypes.Select(t => t.Name))}");
                
                // Scan PlayerPrefs keys for JSON data
                var playerPrefsKeys = this.GetAllPlayerPrefsKeys();
                var jsonKeys = this.FilterJsonKeys(playerPrefsKeys);
                
                Debug.Log($"📋 Found {jsonKeys.Count} potential JSON keys in PlayerPrefs: {string.Join(", ", jsonKeys)}");
                
                // Create entries for available types (whether they have data or not)
                foreach (var type in availableGameDataTypes)
                {
                    var entry = new PlayerPrefsDataEntry(type);
                    entry.OnDataChanged += this.OnDataEntryChanged;
                    this.dataEntries.Add(entry);
                    this.entryMap[type] = entry;
                }
                
                // Try to match JSON keys with types and auto-load data
                this.AutoMatchAndLoadData(jsonKeys, availableGameDataTypes);
                
                this.UpdateUI();
                this.UpdateLastAction($"🔍 Found {availableGameDataTypes.Count} data types, {jsonKeys.Count} JSON keys");
                
                Debug.Log($"✅ Scan completed: {availableGameDataTypes.Count} types, {jsonKeys.Count} JSON keys in PlayerPrefs");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error scanning for game data: {ex.Message}");
                this.UpdateLastAction("❌ Scan failed");
            }
        }
        
        /// <summary>
        /// Gets all available IGameData types from loaded assemblies
        /// </summary>
        private List<Type> GetAvailableGameDataTypes()
        {
            var gameDataTypes = new List<Type>();
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(type => 
                            type.IsClass && 
                            !type.IsAbstract && 
                            typeof(IGameData).IsAssignableFrom(type) &&
                            type.GetCustomAttribute<SerializableAttribute>() != null)
                        .ToArray();
                    
                    gameDataTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogWarning($"⚠️ Could not load types from assembly {assembly.FullName}: {ex.Message}");
                }
            }
            
            return gameDataTypes;
        }
        
        /// <summary>
        /// Gets all PlayerPrefs keys with enhanced cross-platform support
        /// </summary>
        private List<string> GetAllPlayerPrefsKeys()
        {
            var keys = new List<string>();
            
            try
            {
                #if UNITY_EDITOR_WIN
                // Windows: Read from Registry
                try
                {
                    using (var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey($"Software\\{Application.companyName}\\{Application.productName}"))
                    {
                        if (registryKey != null)
                        {
                            var registryKeys = registryKey.GetValueNames();
                            keys.AddRange(registryKeys);
                            Debug.Log($"🔍 Windows Registry scan found {registryKeys.Length} keys");
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ Registry key not found, falling back to pattern matching");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"⚠️ Registry access failed: {ex.Message}, using fallback method");
                }
                #endif
                
                // Fallback: Try common patterns (works on all platforms)
                if (keys.Count == 0)
                {
                    keys.AddRange(this.GetKeysUsingPatternMatching());
                }
                
                // Additional fallback: Check for type-based keys
                var additionalKeys = this.GetTypeBasedKeys();
                foreach (var key in additionalKeys)
                {
                    if (!keys.Contains(key))
                    {
                        keys.Add(key);
                    }
                }
                
                Debug.Log($"🔍 Total PlayerPrefs keys found: {keys.Count}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error getting PlayerPrefs keys: {ex.Message}");
                // Last resort: try type-based keys only
                keys.AddRange(this.GetTypeBasedKeys());
            }
            
            return keys;
        }
        
        /// <summary>
        /// Gets keys using pattern matching (cross-platform fallback)
        /// </summary>
        private List<string> GetKeysUsingPatternMatching()
        {
            var keys = new List<string>();
            
            try
            {
                var commonPrefixes = new[] { 
                    "GameData_", "Data_", "Save_", "Player_", "Game_", "Config_", 
                    "Settings_", "Progress_", "State_", "User_", "Session_"
                };
                
                var commonSuffixes = new[] { 
                    "Data", "Config", "Settings", "Progress", "State", "Info", 
                    "Save", "Profile", "Stats", "Inventory", "Achievement"
                };
                
                var commonNames = new[] {
                    "PlayerData", "GameData", "SaveData", "UserData", "ConfigData",
                    "Settings", "Progress", "Inventory", "Stats", "Profile"
                };
                
                // Check prefixed patterns
                foreach (var prefix in commonPrefixes)
                {
                    foreach (var suffix in commonSuffixes)
                    {
                        var testKey = prefix + suffix;
                        if (PlayerPrefs.HasKey(testKey))
                        {
                            keys.Add(testKey);
                            Debug.Log($"🎯 Found key via pattern: {testKey}");
                        }
                    }
                }
                
                // Check common names
                foreach (var name in commonNames)
                {
                    if (PlayerPrefs.HasKey(name))
                    {
                        keys.Add(name);
                        Debug.Log($"🎯 Found key via common name: {name}");
                    }
                }
                
                Debug.Log($"🔍 Pattern matching found {keys.Count} keys");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"⚠️ Pattern matching failed: {ex.Message}");
            }
            
            return keys;
        }
        
        /// <summary>
        /// Gets keys based on available IGameData types
        /// </summary>
        private List<string> GetTypeBasedKeys()
        {
            var keys = new List<string>();
            
            try
            {
                var gameDataTypes = this.GetAvailableGameDataTypes();
                
                foreach (var type in gameDataTypes)
                {
                    var possibleKeys = this.GeneratePossibleKeysForType(type);
                    
                    foreach (var key in possibleKeys)
                    {
                        if (PlayerPrefs.HasKey(key) && !keys.Contains(key))
                        {
                            keys.Add(key);
                            Debug.Log($"🎯 Found key for type {type.Name}: {key}");
                        }
                    }
                }
                
                Debug.Log($"🔍 Type-based search found {keys.Count} keys");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"⚠️ Type-based key search failed: {ex.Message}");
            }
            
            return keys;
        }
        
        /// <summary>
        /// Generates all possible keys for a specific type
        /// </summary>
        private List<string> GeneratePossibleKeysForType(Type type)
        {
            var typeName = type.Name;
            var keys = new List<string>
            {
                $"GameData_{typeName}",                    // GameData_TypeName
                typeName,                                  // TypeName
                $"Data_{typeName}",                       // Data_TypeName
                $"Save_{typeName}",                       // Save_TypeName
                $"Player_{typeName}",                     // Player_TypeName
                $"Game_{typeName}",                       // Game_TypeName
                $"Config_{typeName}",                     // Config_TypeName
                $"{typeName}Data",                        // TypeNameData
                $"{typeName}Config",                      // TypeNameConfig
                $"{typeName}Settings",                    // TypeNameSettings
            };
            
            // Add variations with common suffixes removed
            var suffixesToTry = new[] { "Data", "Config", "Settings", "Info", "State" };
            foreach (var suffix in suffixesToTry)
            {
                if (typeName.EndsWith(suffix))
                {
                    var baseName = typeName.Substring(0, typeName.Length - suffix.Length);
                    keys.Add(baseName);
                    keys.Add($"GameData_{baseName}");
                    keys.Add($"Data_{baseName}");
                    keys.Add($"Save_{baseName}");
                }
            }
            
            return keys.Distinct().ToList();
        }
        
        /// <summary>
        /// Filters keys that likely contain JSON data with enhanced validation
        /// </summary>
        private List<string> FilterJsonKeys(List<string> allKeys)
        {
            var jsonKeys = new List<string>();
            
            Debug.Log($"🔍 Filtering {allKeys.Count} keys for JSON content...");
            
            foreach (var key in allKeys)
            {
                try
                {
                    var value = PlayerPrefs.GetString(key, "");
                    
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }
                    
                    if (this.IsValidJson(value))
                    {
                        jsonKeys.Add(key);
                        Debug.Log($"🔍 Found JSON key: '{key}' ({value.Length} chars)");
                        
                        // Log a preview of the JSON content
                        var preview = value.Length > 100 ? value.Substring(0, 100) + "..." : value;
                        Debug.Log($"📄 JSON preview: {preview}");
                    }
                    else
                    {
                        Debug.Log($"⚠️ Key '{key}' contains non-JSON data: {value.Substring(0, Math.Min(50, value.Length))}...");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"⚠️ Could not read PlayerPrefs key '{key}': {ex.Message}");
                }
            }
            
            Debug.Log($"✅ Found {jsonKeys.Count} JSON keys out of {allKeys.Count} total keys");
            return jsonKeys;
        }
        
        /// <summary>
        /// Checks if a string is valid JSON with enhanced validation
        /// </summary>
        private bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                return false;
            
            jsonString = jsonString.Trim();
            
            // Quick format check - must start/end with appropriate brackets
            if (!((jsonString.StartsWith("{") && jsonString.EndsWith("}")) ||
                  (jsonString.StartsWith("[") && jsonString.EndsWith("]"))))
            {
                return false;
            }
            
            try
            {
                // Use JsonConvert with proper settings
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => args.ErrorContext.Handled = true
                };
                
                var obj = JsonConvert.DeserializeObject(jsonString, settings);
                
                // Additional validation - ensure it's not just primitive values
                if (obj == null)
                    return false;
                
                // Accept objects and arrays, but not simple primitives stored as JSON
                return obj is Newtonsoft.Json.Linq.JObject || obj is Newtonsoft.Json.Linq.JArray;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"⚠️ Unexpected error validating JSON: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Attempts to match JSON keys with available types and auto-load compatible data
        /// </summary>
        private void AutoMatchAndLoadData(List<string> jsonKeys, List<Type> availableTypes)
        {
            foreach (var key in jsonKeys)
            {
                try
                {
                    var jsonData = PlayerPrefs.GetString(key);
                    var matchedType = this.TryMatchJsonToType(jsonData, availableTypes, key);
                    
                    if (matchedType != null && this.entryMap.ContainsKey(matchedType))
                    {
                        Debug.Log($"✅ Auto-matched key '{key}' to type '{matchedType.Name}'");
                        // The PlayerPrefsDataEntry will handle loading when LoadData() is called
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Could not match JSON key '{key}' to any available IGameData type");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"⚠️ Error processing key '{key}': {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Tries to match JSON data to an available IGameData type with enhanced matching
        /// </summary>
        private Type TryMatchJsonToType(string jsonData, List<Type> availableTypes, string key)
        {
            Debug.Log($"🎯 Attempting to match key '{key}' with available types...");
            
            // First try: Match by key naming convention
            var typeByKey = this.TryMatchByKeyName(key, availableTypes);
            if (typeByKey != null)
            {
                // Verify the JSON can actually be deserialized to this type
                if (this.CanDeserializeToType(jsonData, typeByKey))
                {
                    Debug.Log($"✅ Matched by key name: '{key}' → {typeByKey.Name}");
                    return typeByKey;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Key '{key}' suggests type {typeByKey.Name} but JSON doesn't match");
                }
            }
            
            // Second try: Attempt deserialization with each type
            foreach (var type in availableTypes)
            {
                if (this.CanDeserializeToType(jsonData, type))
                {
                    Debug.Log($"✅ Matched by deserialization: '{key}' → {type.Name}");
                    return type;
                }
            }
            
            Debug.LogWarning($"❌ Could not match key '{key}' to any available type");
            return null;
        }
        
        /// <summary>
        /// Tries to match a key name to a type
        /// </summary>
        private Type TryMatchByKeyName(string key, List<Type> availableTypes)
        {
            // Direct matches
            var directMatches = new Dictionary<string, Func<Type, bool>>
            {
                // Standard patterns
                ["GameData_"] = type => key.Equals($"GameData_{type.Name}", StringComparison.OrdinalIgnoreCase),
                ["Data_"] = type => key.Equals($"Data_{type.Name}", StringComparison.OrdinalIgnoreCase),
                ["Save_"] = type => key.Equals($"Save_{type.Name}", StringComparison.OrdinalIgnoreCase),
                ["Player_"] = type => key.Equals($"Player_{type.Name}", StringComparison.OrdinalIgnoreCase),
                ["Game_"] = type => key.Equals($"Game_{type.Name}", StringComparison.OrdinalIgnoreCase),
                ["Config_"] = type => key.Equals($"Config_{type.Name}", StringComparison.OrdinalIgnoreCase),
                
                // Direct type name
                [""] = type => key.Equals(type.Name, StringComparison.OrdinalIgnoreCase),
                
                // Suffix patterns
                ["Data"] = type => key.Equals($"{type.Name}Data", StringComparison.OrdinalIgnoreCase),
                ["Config"] = type => key.Equals($"{type.Name}Config", StringComparison.OrdinalIgnoreCase),
                ["Settings"] = type => key.Equals($"{type.Name}Settings", StringComparison.OrdinalIgnoreCase),
            };
            
            foreach (var pattern in directMatches)
            {
                var matchingType = availableTypes.FirstOrDefault(pattern.Value);
                if (matchingType != null)
                {
                    return matchingType;
                }
            }
            
            // Try removing common suffixes from type names
            foreach (var type in availableTypes)
            {
                var typeName = type.Name;
                var suffixesToRemove = new[] { "Data", "Config", "Settings", "Info", "State" };
                
                foreach (var suffix in suffixesToRemove)
                {
                    if (typeName.EndsWith(suffix))
                    {
                        var baseName = typeName.Substring(0, typeName.Length - suffix.Length);
                        if (key.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                            key.Equals($"GameData_{baseName}", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals($"Data_{baseName}", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals($"Save_{baseName}", StringComparison.OrdinalIgnoreCase))
                        {
                            return type;
                        }
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Safely checks if JSON can be deserialized to a specific type
        /// </summary>
        private bool CanDeserializeToType(string jsonData, Type targetType)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => args.ErrorContext.Handled = true,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                var deserializedObj = JsonConvert.DeserializeObject(jsonData, targetType, settings);
                return deserializedObj != null;
            }
            catch (Exception ex)
            {
                Debug.Log($"🔍 Cannot deserialize to {targetType.Name}: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Updates the UI based on current state
        /// </summary>
        private void UpdateUI()
        {
            this.dataContainer?.Clear();
            
            if (this.dataEntries.Count == 0)
            {
                this.ShowEmptyState();
            }
            else
            {
                this.ShowDataEntries();
            }
            
            this.UpdateDataCount();
        }
        
        /// <summary>
        /// Shows empty state when no data types are found
        /// </summary>
        private void ShowEmptyState()
        {
            if (this.emptyState != null)
            {
                this.emptyState.style.display = DisplayStyle.Flex;
            }
            
            if (this.dataScrollView != null)
            {
                this.dataScrollView.style.display = DisplayStyle.None;
            }
        }
        
        /// <summary>
        /// Shows data entries in the scroll view
        /// </summary>
        private void ShowDataEntries()
        {
            if (this.emptyState != null)
            {
                this.emptyState.style.display = DisplayStyle.None;
            }
            
            if (this.dataScrollView != null)
            {
                this.dataScrollView.style.display = DisplayStyle.Flex;
            }
            
            foreach (var entry in this.dataEntries)
            {
                var entryUI = entry.CreateUI();
                this.dataContainer?.Add(entryUI);
            }
        }
        
        /// <summary>
        /// Updates the data count label
        /// </summary>
        private void UpdateDataCount()
        {
            if (this.dataCountLabel != null)
            {
                var hasDataCount = this.dataEntries.Count(entry => entry.HasData);
                this.dataCountLabel.text = $"📊 Found {this.dataEntries.Count} data types ({hasDataCount} with data)";
            }
        }
        
        /// <summary>
        /// Updates the last action status label
        /// </summary>
        private void UpdateLastAction(string message)
        {
            if (this.lastActionLabel != null)
            {
                this.lastActionLabel.text = message;
            }
        }
        
        /// <summary>
        /// Loads all data from PlayerPrefs with enhanced scanning and loading
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                Debug.Log("🔄 Starting Load All Data operation...");
                
                // First, rescan for any new data
                this.ScanForGameDataTypes();
                
                var loadedCount = 0;
                var errorCount = 0;
                var foundDataCount = 0;
                
                Debug.Log($"📋 Attempting to load data for {this.dataEntries.Count} data types...");
                
                foreach (var entry in this.dataEntries)
                {
                    try
                    {
                        Debug.Log($"🔍 Loading data for {entry.TypeName}...");
                        entry.LoadData();
                        
                        if (entry.HasData)
                        {
                            loadedCount++;
                            foundDataCount++;
                            Debug.Log($"✅ Successfully loaded data for {entry.TypeName}");
                        }
                        else
                        {
                            Debug.Log($"⚠️ No data found for {entry.TypeName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Error loading {entry.TypeName}: {ex.Message}");
                        errorCount++;
                    }
                }
                
                // Force UI refresh
                this.RefreshUI();
                this.UpdateDataCount();
                
                if (errorCount > 0)
                {
                    this.UpdateLastAction($"⚠️ Loaded {loadedCount} entries with {errorCount} errors");
                }
                else if (foundDataCount > 0)
                {
                    this.UpdateLastAction($"✅ Loaded {loadedCount} data entries successfully");
                }
                else
                {
                    this.UpdateLastAction($"📭 No saved data found - all entries show defaults");
                }
                
                Debug.Log($"✅ Load All Data completed: {loadedCount} loaded, {errorCount} errors, {foundDataCount} with actual data");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in LoadAllData: {ex.Message}\n{ex.StackTrace}");
                this.UpdateLastAction("❌ Load all failed");
            }
        }
        
        /// <summary>
        /// Forces a complete UI refresh
        /// </summary>
        private void RefreshUI()
        {
            try
            {
                Debug.Log("🔄 Refreshing UI...");
                
                // Clear current UI
                this.dataContainer?.Clear();
                
                // Rebuild UI with current data entries
                this.UpdateUI();
                
                Debug.Log("✅ UI refreshed successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error refreshing UI: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Saves all data to PlayerPrefs
        /// </summary>
        private void SaveAllData()
        {
            try
            {
                var savedCount = 0;
                var errorCount = 0;
                
                foreach (var entry in this.dataEntries)
                {
                    try
                    {
                        if (entry.HasData)
                        {
                            entry.SaveData();
                            savedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Error saving {entry.TypeName}: {ex.Message}");
                        errorCount++;
                    }
                }
                
                if (errorCount > 0)
                {
                    this.UpdateLastAction($"⚠️ Saved {savedCount} entries with {errorCount} errors");
                }
                else
                {
                    this.UpdateLastAction($"✅ Saved {savedCount} data entries successfully");
                }
                
                Debug.Log($"✅ Save All Data completed: {savedCount} saved, {errorCount} errors");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in SaveAllData: {ex.Message}");
                this.UpdateLastAction("❌ Save all failed");
            }
        }
        
        /// <summary>
        /// Shows confirmation dialog before deleting all data
        /// </summary>
        private void ShowDeleteAllConfirmation()
        {
            var result = EditorUtility.DisplayDialog(
                "🗑️ Delete All PlayerPrefs Data",
                "Are you sure you want to delete ALL PlayerPrefs data?\n\nThis action cannot be undone!",
                "Delete All",
                "Cancel"
            );
            
            if (result)
            {
                this.DeleteAllData();
            }
        }
        
        /// <summary>
        /// Deletes all PlayerPrefs data
        /// </summary>
        private void DeleteAllData()
        {
            try
            {
                var deletedCount = 0;
                var errorCount = 0;
                
                foreach (var entry in this.dataEntries)
                {
                    try
                    {
                        entry.DeleteData();
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Error deleting {entry.TypeName}: {ex.Message}");
                        errorCount++;
                    }
                }
                
                // Also clear all PlayerPrefs as fallback
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                
                this.UpdateDataCount();
                
                if (errorCount > 0)
                {
                    this.UpdateLastAction($"⚠️ Deleted {deletedCount} entries with {errorCount} errors");
                }
                else
                {
                    this.UpdateLastAction($"🗑️ All PlayerPrefs data cleared successfully");
                }
                
                Debug.Log($"✅ Delete All Data completed: {deletedCount} deleted, {errorCount} errors");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in DeleteAllData: {ex.Message}");
                this.UpdateLastAction("❌ Delete all failed");
            }
        }
        
        /// <summary>
        /// Called when a data entry is changed by the user
        /// </summary>
        private void OnDataEntryChanged(PlayerPrefsDataEntry entry)
        {
            this.UpdateDataCount();
            this.UpdateLastAction($"✏️ Modified {entry.TypeName}");
        }
        
        /// <summary>
        /// Refreshes the tool by rescanning for data types
        /// </summary>
        [MenuItem("Tools/Foundations/Refresh PlayerPrefs Data Manager", false, 101)]
        public static void RefreshTool()
        {
            var window = GetWindow<PlayerPrefsDataTool>(false, null, false);
            if (window != null)
            {
                window.ScanForGameDataTypes();
            }
        }
        
        /// <summary>
        /// Opens PlayerPrefs in the system (Windows only)
        /// </summary>
        [MenuItem("Tools/Foundations/Open PlayerPrefs Location", false, 102)]
        public static void OpenPlayerPrefsLocation()
        {
            try
            {
                #if UNITY_EDITOR_WIN
                var companyName = Application.companyName;
                var productName = Application.productName;
                var registryPath = $"HKEY_CURRENT_USER\\Software\\{companyName}\\{productName}";
                
                EditorUtility.DisplayDialog(
                    "📂 PlayerPrefs Location", 
                    $"PlayerPrefs are stored in Windows Registry at:\n\n{registryPath}\n\nYou can open Registry Editor (regedit) to view them manually.",
                    "OK"
                );
                #elif UNITY_EDITOR_OSX
                var path = $"~/Library/Preferences/unity.{Application.companyName}.{Application.productName}.plist";
                EditorUtility.DisplayDialog(
                    "📂 PlayerPrefs Location", 
                    $"PlayerPrefs are stored at:\n\n{path}",
                    "OK"
                );
                #else
                EditorUtility.DisplayDialog(
                    "📂 PlayerPrefs Location", 
                    "PlayerPrefs location varies by platform. This feature is only available on Windows and Mac.",
                    "OK"
                );
                #endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error showing PlayerPrefs location: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Debug tool to scan and log all PlayerPrefs keys and content
        /// </summary>
        [MenuItem("Tools/Foundations/Debug PlayerPrefs Scanner", false, 103)]
        public static void DebugPlayerPrefsScanner()
        {
            try
            {
                Debug.Log("🔍 === DEBUG PLAYERPREFS SCANNER ===");
                
                var tool = new PlayerPrefsDataTool();
                
                // Scan for types
                var types = tool.GetAvailableGameDataTypes();
                Debug.Log($"📦 Found {types.Count} IGameData types: {string.Join(", ", types.Select(t => t.Name))}");
                
                // Scan for keys
                var allKeys = tool.GetAllPlayerPrefsKeys();
                Debug.Log($"🔑 Found {allKeys.Count} total PlayerPrefs keys");
                
                foreach (var key in allKeys)
                {
                    try
                    {
                        var value = PlayerPrefs.GetString(key, "");
                        var isJson = tool.IsValidJson(value);
                        var preview = value.Length > 100 ? value.Substring(0, 100) + "..." : value;
                        
                        Debug.Log($"📄 Key: '{key}' | JSON: {isJson} | Length: {value.Length} | Preview: {preview}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"⚠️ Error reading key '{key}': {ex.Message}");
                    }
                }
                
                // Try to match JSON keys with types
                var jsonKeys = tool.FilterJsonKeys(allKeys);
                Debug.Log($"📋 Found {jsonKeys.Count} JSON keys");
                
                foreach (var jsonKey in jsonKeys)
                {
                    var jsonData = PlayerPrefs.GetString(jsonKey);
                    var matchedType = tool.TryMatchJsonToType(jsonData, types, jsonKey);
                    
                    if (matchedType != null)
                    {
                        Debug.Log($"✅ Successfully matched '{jsonKey}' → {matchedType.Name}");
                    }
                    else
                    {
                        Debug.Log($"❌ Could not match '{jsonKey}' to any type");
                    }
                }
                
                Debug.Log("🔍 === END DEBUG SCANNER ===");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Debug scanner failed: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        private void OnDisable()
        {
            // Clean up event handlers
            foreach (var entry in this.dataEntries)
            {
                entry.OnDataChanged -= this.OnDataEntryChanged;
            }
        }
    }
}
