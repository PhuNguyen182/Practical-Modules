using PracticalSystems.GameResourceSystem.Models;
using UnityEngine;

namespace PracticalSystems.InventorySystem.Models.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Inventory/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] public string itemId;
        [SerializeField] public string itemName;
        [SerializeField] public string itemDescription;
        [SerializeField] public string iconName;
        [SerializeField] public string prefabName;
        [SerializeField] public string setId;
        [SerializeField] public ItemCategory itemCategory;
        [SerializeField] public ResourceData[] buyPricing;
        [SerializeField] public ResourceData[] sellPricing;
    }
}
