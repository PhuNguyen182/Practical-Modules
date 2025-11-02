using System;
using System.Collections.Generic;
using UnityEngine;

namespace PracticalSystems.InventorySystem.Models.Set
{
    [Serializable]
    public class ItemSetData
    {
        [SerializeField] public string setId;
        [SerializeField] public string setName;
        [SerializeField] public string setDescription;
        [SerializeField] public string setIconName;
        [SerializeField] public List<string> requiredItemIds;
    }
}
