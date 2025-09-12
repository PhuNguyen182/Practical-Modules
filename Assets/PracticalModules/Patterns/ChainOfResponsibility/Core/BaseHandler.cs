namespace PracticalModules.Patterns.ChainOfResponsibility.Core
{
    public abstract class BaseHandler<TRequest, TResponse> : IHandler<TRequest, TResponse> 
    {
        private IHandler<TRequest, TResponse> _nextHandler;

        public abstract bool CanHandle(TRequest request);
        
        public IHandler<TRequest, TResponse> SetNext(IHandler<TRequest, TResponse> handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual TResponse Handle(TRequest request)
        {
            if (_nextHandler != null)
                return _nextHandler.Handle(request);

            return default;
        }
    }
} 