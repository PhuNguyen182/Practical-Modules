using PracticalSystems.GameResourceSystem.Models;

namespace PracticalSystems.GameResourceSystem.Handlers
{
    public class CoinResourceHandler : BaseGameResourceHandler
    {
        public override GameResourceType ResourceType => GameResourceType.Coin;

        public CoinResourceHandler(ResourceData resourceData) : base(resourceData)
        {
            this.MigrateOldData();
        }

        public sealed override void MigrateOldData()
        {
            
        }

        public override void SpendResources(int amount)
        {
            this.ResourceData.amount -= amount;
        }
    }
}
