using System;
using Newtonsoft.Json;
using UnityEngine;
using Foundations.DataFlow.MicroData;

namespace Foundations.DataFlow.Examples
{
    /// <summary>
    /// Simple test data class for debugging the PlayerPrefs Data Manager tool
    /// This ensures the tool always has at least one data type to work with
    /// </summary>
    [Serializable]
    public class SimpleTestData : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        [Header("Basic Fields")]
        public string testString = "Test Value";
        public int testInt = 42;
        public float testFloat = 3.14f;
        public bool testBool = true;
        
        [Header("Unity Types")]
        public Vector2 testVector2 = Vector2.one;
        public Vector3 testVector3 = Vector3.up;
        public Color testColor = Color.green;
        
        public SimpleTestData()
        {
            // Default constructor for tool instantiation
        }
        
        public SimpleTestData(string str, int num, float dec, bool flag)
        {
            this.testString = str;
            this.testInt = num;
            this.testFloat = dec;
            this.testBool = flag;
        }
    }
    
    /// <summary>
    /// Another simple test data class for testing multiple entries
    /// </summary>
    [Serializable]
    public class AnotherTestData : IGameData
    {
        [JsonIgnore]
        public int Version => 1;
        
        public string playerName = "Default Player";
        public int score = 1000;
        public bool isActive = false;
    }
}
