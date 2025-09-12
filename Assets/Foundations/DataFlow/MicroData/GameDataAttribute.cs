using System;

namespace Foundations.DataFlow.MicroData
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameDataAttribute : Attribute
    {
        public string DataKey { get; }
        
        public GameDataAttribute(string dataKey)
        {
            DataKey = dataKey;
        }
    }
}
