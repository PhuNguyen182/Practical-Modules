using System;
using System.Collections.Generic;
using Foundations.DataFlow.MicroData;

namespace PracticalModules.GameResourceSystem.Models
{
    [Serializable]
    [GameData(nameof(GameResourceProgressData))]
    public class GameResourceProgressData : IGameData
    {
        public int Version { get; set; }
        public Dictionary<GameResourceType, ResourceData> GameResourceData = new();
    }
}
