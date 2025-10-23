using PracticalModules.GameResourceSystem.Models;

namespace PracticalModules.GameResourceSystem.Handlers
{
    public abstract class BaseGameResourceHandler
    {
        protected readonly ResourceData ResourceData;
        public abstract GameResourceType ResourceType { get; }

        protected BaseGameResourceHandler(ResourceData resourceData)
        {
            this.ResourceData = resourceData;
        }
        
        public abstract void MigrateOldData();
        
        public int GetResourceAmount() => this.ResourceData.amount;
        
        public void SetResourceAmount(int amount) => this.ResourceData.amount = amount;
        
        public virtual void EarnResources(int amount)
        {
            this.ResourceData.amount += amount;
        }

        public virtual bool CanSpendResources(int amount)
        {
            return this.ResourceData.amount >= amount;
        }

        public abstract void SpendResources(int amount);
    }
}
