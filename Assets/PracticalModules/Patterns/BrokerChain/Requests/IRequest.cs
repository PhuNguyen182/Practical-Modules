namespace PracticalModules.Patterns.BrokerChain.Requests
{
    public interface IRequest
    {
        public string Id { get; }
        public bool IsCompleted { get; set; }
        
        public void Handle();
    }
}
