using Foundations.DataFlow.MicroData.StaticDataControllers;

namespace Foundations.DataFlow.MasterDataController
{
    public interface IStaticCustomDataManager : ICustomDataManager
    {
        public TDataHandler? GetDataHandler<TDataHandler>() where TDataHandler : class, IStaticGameDataHandler;
    }
}