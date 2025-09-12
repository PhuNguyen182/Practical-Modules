namespace PracticalModules.Patterns.Visitors.Core
{
    public interface IVisitor
    {
        public void Visit(IVisitable visitable);
    }
}
