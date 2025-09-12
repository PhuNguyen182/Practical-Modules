using System.Collections;
using System.Collections.Generic;
using PracticalModules.MessageBrokers.MessageTypes;
using PracticalUtilities.DebugUtils;
using UnityEngine;

public struct TestAsyncMessageData : IAsyncMessageData
{
    
}

public class TestAsyncMessage : AsyncMessageType<TestAsyncMessageData>
{
    private void Awake()
    {
        
    }
}
