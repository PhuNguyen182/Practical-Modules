using System;
using System.Collections.Generic;
using PracticalModules.Patterns.BrokerChain.Handlers;
using PracticalModules.Patterns.BrokerChain.Requests;

namespace PracticalModules.Patterns.BrokerChain.Mediator
{
    public abstract class BrokerChainMediator : IMediator, IDisposable
    {
        private readonly List<IRequestHandler> _handlers = new();
        private readonly Dictionary<Type, List<IRequestHandler>> _cachedHandlers = new();
        private readonly RequestHandlerComparer _handlerComparer = new();

        public void RegisterHandler(IRequestHandler handler)
        {
            if (handler == null) 
                return;

            handler.OnComplete += UnregisterHandler;
            _handlers.Add(handler);
            _handlers.Sort(_handlerComparer);
            _cachedHandlers.Clear();
        }

        public void UnregisterHandler(IRequestHandler handler)
        {
            if (handler == null) 
                return;

            _handlers.Remove(handler);
            _cachedHandlers.Clear();
        }

        public void ProcessAllRequests()
        {
            
        }

        public void ProcessRequest(IRequest request)
        {
            if (request == null) 
                return;
            
            Type requestType = request.GetType();
            
            if (!_cachedHandlers.ContainsKey(requestType))
            {
                List<IRequestHandler> compatibleHandlers = new();
                foreach (IRequestHandler handler in _handlers)
                {
                    if (handler.CanHandle(request))
                        compatibleHandlers.Add(handler);
                }
                
                _cachedHandlers[requestType] = compatibleHandlers;
            }

            List<IRequestHandler> handlersToUse = _cachedHandlers[requestType];
            foreach (IRequestHandler handler in handlersToUse)
            {
                handler.Handle(request, this);
                if (request.IsCompleted)
                    break;
            }
        }

        public void Dispose()
        {
            _handlers.Clear();
            _cachedHandlers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}