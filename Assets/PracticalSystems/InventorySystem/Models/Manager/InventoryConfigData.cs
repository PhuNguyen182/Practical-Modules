using System;
using Foundations.DataFlow.MicroData;
using Newtonsoft.Json;
using PracticalSystems.InventorySystem.Models.Items;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using UnityEngine;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    [GameData(nameof(InventoryConfigData))]
    [CreateAssetMenu(fileName = "InventoryConfigData",
        menuName = "Scriptable Objects/Inventory/ConfigData/InventoryConfigData")]
    public class InventoryConfigData : ScriptableObject, IGameData
    {
        public int Version { get; set; }
        public InventoryCategoryItemDictionary inventoryCategoryItemDatabase = new();

#if UNITY_EDITOR
        [SerializeField] private string jsonData;

        [Button]
        public void SerializeToJson()
        {
            this.jsonData = "";
            string json = JsonConvert.SerializeObject(this);
            this.jsonData = json;
            Debug.Log(this.jsonData);
        }
#endif
    }

    [Serializable]
    public class InventoryCategoryItemDictionary : SerializedDictionary<ItemCategory, InventoryCategoryItemDatabase>
    {

    }
}
