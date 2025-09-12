using PracticalModules.Patterns.BrokerChain.Handlers;
using PracticalModules.Patterns.BrokerChain.Requests;

namespace PracticalModules.Patterns.BrokerChain.Mediator
{
    public interface IMediator
    {
        public void RegisterHandler(IRequestHandler handler);
        public void UnregisterHandler(IRequestHandler handler);
        public void ProcessAllRequests();
        public void ProcessRequest(IRequest request);
    }
}