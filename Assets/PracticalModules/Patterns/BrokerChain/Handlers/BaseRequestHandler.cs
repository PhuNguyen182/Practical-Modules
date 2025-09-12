using System;
using PracticalModules.Patterns.BrokerChain.Mediator;
using PracticalModules.Patterns.BrokerChain.Requests;

namespace PracticalModules.Patterns.BrokerChain.Handlers
{
    public abstract class BaseRequestHandler : IRequestHandler
    {
        public abstract int Priority { get; }
        
        public Action<IRequestHandler> OnComplete { get; set; }
        
        public abstract void SetPriority(int priority);
        public abstract void Handle(IRequest request, IMediator mediator);
        public abstract bool CanHandle(IRequest request);
        
        public void Dispose()
        {
            OnComplete?.Invoke(this);
            OnComplete = null;
            GC.SuppressFinalize(this);
        }
    }
}