using System;
using System.Collections.Generic;
using Foundations.DataFlow.MicroData;
using Newtonsoft.Json;
using PracticalModules.MessageBrokers.Core;
using PracticalModules.Temps.Scripts;
using UnityEngine;
using MessagePipe;

public class SampleClass : IDisposable
{
    private bool _disposed;
    private YourClass _yourClass;
    private readonly List<int> _numbers;
    
    public SampleClass()
    {
        _numbers = new();
    }
    
    ~SampleClass()
    {
        Debug.Log("Destructor called.");
        Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
            _numbers.Clear();
        
        _disposed = true;
    }
}

[Serializable]
public class YourClass : IGameData
{
    public int number;
    public string text;
    public bool flag;
    public int Version { get; }
}

[Serializable]
public class YourClass2 : IGameData
{
    public int age;
    public string name;
    public float score;
    public int Version { get; }
}

public class MessageBrokerExample : MonoBehaviour
{
    private SampleClass _sampleClass;
    public bool saveCheck;
    public bool loadCheck;
    
    private void Awake()
    {
        _sampleClass = new();
    }
    
    private void Start()
    {
        
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (saveCheck)
        {
            saveCheck = false;
            YourClass yourClass = new()
            {
                number = 1,
                text = "text",
                flag = true
            };
            
            var yourClass2 = new YourClass2
            {
                age = 3,
                name = "Fuss",
                score = 1.35f
            };
            
            string yourClassData = JsonConvert.SerializeObject(yourClass);
            string yourClassData2 = JsonConvert.SerializeObject(yourClass2);
            PlayerPrefs.SetString(nameof(YourClass), yourClassData);
            PlayerPrefs.SetString(nameof(YourClass2), yourClassData2);
        }

        if (loadCheck)
        {
            loadCheck = false;
            string yourClassData = PlayerPrefs.GetString(nameof(YourClass));
            string yourClassData2 = PlayerPrefs.GetString(nameof(YourClass2));
            YourClass yourClass = JsonConvert.DeserializeObject<YourClass>(yourClassData);
            YourClass2 yourClass2 = JsonConvert.DeserializeObject<YourClass2>(yourClassData2);
            Debug.Log(yourClass.number);
            Debug.Log(yourClass2.age);
        }
    }
#endif

    private void OnDestroy()
    {
        _sampleClass.Dispose();
    }
}
