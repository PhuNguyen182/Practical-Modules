using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MasterDataController;

namespace Foundations.DataFlow.MicroData.DynamicDataControllers
{
    public interface IDynamicGameDataHandler
    {
        public void Initialize();
        public void InjectDataManager(IMainDataManager mainDataManager);
        public UniTask Load();
        public UniTask SaveAsync();
        public void Save();
        public void Delete();
    }
}
