using Cysharp.Threading.Tasks;

namespace PracticalModules.MessageBrokers.MessageTypes
{
    public abstract class AsyncMessageType<TMessageData> : IMessageType where TMessageData : IAsyncMessageData
    {
        public TMessageData MessageData;
        public UniTaskCompletionSource<TMessageData> CompletionSource;
    }
}
