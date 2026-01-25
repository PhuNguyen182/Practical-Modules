using PracticalModules.MessageBrokers.MessageTypes;

public struct TestAsyncMessageData : IAsyncMessageData
{
    
}

[MessageBroker]
public class TestAsyncMessage : AsyncMessageType<TestAsyncMessageData>
{
    private void Awake()
    {
        
    }
}
