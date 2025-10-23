using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.MicroData.DynamicDataControllers
{
    public interface IDynamicGameDataHandler
    {
        public void Initialize();
        public UniTask Load();
        public UniTask SaveAsync();
        public void Save();
        public void Delete();
    }
}
