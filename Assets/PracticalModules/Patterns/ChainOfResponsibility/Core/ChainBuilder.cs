using System.Collections.Generic;
using UnityEngine;

namespace PracticalModules.Patterns.ChainOfResponsibility.Core
{
    public class ChainBuilder<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        private readonly List<IHandler<TRequest, TResponse>> _handlers = new();

        public ChainBuilder<TRequest, TResponse> AddHandler(IHandler<TRequest, TResponse> handler)
        {
            _handlers.Add(handler);
            return this;
        }

        public IHandler<TRequest, TResponse> Build()
        {
            if (_handlers.Count == 0)
            {
                Debug.LogWarning("No handlers added to the chain");
                return null;
            }

            for (int i = 0; i < _handlers.Count - 1; i++)
                _handlers[i].SetNext(_handlers[i + 1]);

            return _handlers[0];
        }

        public static ChainBuilder<TRequest, TResponse> Create() => new();
    }
} 