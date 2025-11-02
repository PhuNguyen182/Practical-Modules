using System;

namespace PracticalSystems.GameResourceSystem.Models
{
    [Serializable]
    public class ResourceData
    {
        public GameResourceType resourceType;
        public int amount;
    }
}