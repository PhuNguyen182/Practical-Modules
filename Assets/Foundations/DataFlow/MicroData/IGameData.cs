using Newtonsoft.Json;

namespace Foundations.DataFlow.MicroData
{
    public interface IGameData
    {
        [JsonIgnore] 
        public int Version { get; }
    }
}
