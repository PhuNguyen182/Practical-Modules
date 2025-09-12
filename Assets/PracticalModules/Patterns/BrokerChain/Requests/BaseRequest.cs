using System;

namespace PracticalModules.Patterns.BrokerChain.Requests
{
    public abstract class BaseRequest : IRequest
    {
        public string Id { get; }
        public bool IsCompleted { get; set; }

        protected BaseRequest()
        {
            Id = Guid.NewGuid().ToString();
            IsCompleted = false;
        }
        
        public abstract void Handle();
    }
}
