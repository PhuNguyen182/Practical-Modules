using System;
using PracticalModules.Patterns.BrokerChain.Mediator;
using PracticalModules.Patterns.BrokerChain.Requests;

namespace PracticalModules.Patterns.BrokerChain.Handlers
{
    public interface IRequestHandler : IDisposable
    {
        public int Priority { get; }
        public Action<IRequestHandler> OnComplete { get; set; }
        
        public bool CanHandle(IRequest request);
        public void Handle(IRequest request, IMediator mediator);
        public void SetPriority(int priority);
    }
}