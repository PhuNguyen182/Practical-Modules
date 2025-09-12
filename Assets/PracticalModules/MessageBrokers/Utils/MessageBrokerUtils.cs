using System;
using Cysharp.Threading.Tasks;
using PracticalModules.MessageBrokers.MessageTypes;
using MessagePipe;

namespace PracticalModules.MessageBrokers.Utils
{
    public struct MessageBrokerUtils<TAsyncMessageData, TAsyncMessage>
        where TAsyncMessageData : IAsyncMessageData
        where TAsyncMessage : AsyncMessageType<TAsyncMessageData>
    {
        public static UniTask<TAsyncMessageData> PublishAsyncMessage(IPublisher<TAsyncMessage> publisher,
            TAsyncMessageData messageData)
        {
            TAsyncMessage message = Activator.CreateInstance<TAsyncMessage>();
            message.MessageData = messageData;
            message.CompletionSource = new();

            publisher.Publish(message);
            return message.CompletionSource.Task;
        }

        public static bool SendBackMessage(AsyncMessageType<TAsyncMessageData> message, TAsyncMessageData data) =>
            message.CompletionSource?.TrySetResult(data) ?? false;
    }
}
