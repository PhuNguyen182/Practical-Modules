namespace PracticalModules.Patterns.Visitors.Core
{
    public interface IVisitable
    {
        public void Accept(IVisitor visitor);
    }
}
