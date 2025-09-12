using System;
using MessagePipe;

namespace PracticalModules.MessageBrokers.MessageFilters
{
    public class PredicateFilter<T> : MessageHandlerFilter<T>
    {
        private readonly Func<T, bool> _predicate;

        public PredicateFilter(Func<T, bool> predicate) => _predicate = predicate;

        public override void Handle(T message, Action<T> next)
        {
            if (_predicate(message))
                next(message);
        }
    }
}
