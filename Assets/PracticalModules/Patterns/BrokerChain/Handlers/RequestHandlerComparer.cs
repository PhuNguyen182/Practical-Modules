using System.Collections.Generic;

namespace PracticalModules.Patterns.BrokerChain.Handlers
{
    public class RequestHandlerComparer : IComparer<IRequestHandler>
    {
        public int Compare(IRequestHandler x, IRequestHandler y)
        {
            if (x == null || y == null)
                return 0;
        
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
