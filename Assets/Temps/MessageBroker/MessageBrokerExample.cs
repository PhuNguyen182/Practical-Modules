using System;
using System.Collections.Generic;
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

public class MessageBrokerExample : MonoBehaviour
{
    private SampleClass _sampleClass;
    
    private void Awake()
    {
        _sampleClass = new();
    }
    
    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        _sampleClass.Dispose();
    }
}
