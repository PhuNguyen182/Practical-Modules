using PracticalModules.MessageBrokers.MessageTypes;

public struct TestAsyncMessageData : IAsyncMessageData
{
    
}

public class TestAsyncMessage : AsyncMessageType<TestAsyncMessageData>
{
    private void Awake()
    {
        
    }
}
