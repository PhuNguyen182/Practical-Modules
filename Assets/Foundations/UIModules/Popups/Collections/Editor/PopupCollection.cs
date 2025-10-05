using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Foundations.UIModules.Popups.Collections.Editor
{
    [CreateAssetMenu(fileName = "PopupCollection", menuName = "Scriptable Objects/Popups/PopupCollection")]
    public class PopupCollection : ScriptableObject
    {
        [SerializeField] public List<PopupRecord> popupRecords;

        [Button]
        private void SetupPopupRecords()
        {
#if UNITY_EDITOR
            this.SetupAddressablePopups();
#endif
        }

#if UNITY_EDITOR
        private void SetupAddressablePopups()
        {
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (addressableSettings == null)
            {
                Debug.LogError("Addressable Settings not found. Please create Addressable Settings first.");
                return;
            }

            // Tìm hoặc tạo group "Popups"
            var popupsGroup = this.FindOrCreatePopupsGroup(addressableSettings);
            if (popupsGroup == null)
            {
                Debug.LogError("Failed to find or create Popups group.");
                return;
            }

            // Thiết lập tất cả popup records thành Addressable
            foreach (var popupRecord in this.popupRecords)
            {
                if (popupRecord?.popupPrefab == null)
                {
                    Debug.LogWarning($"PopupRecord has null prefab, skipping...");
                    continue;
                }

                this.SetupPopupAsAddressable(popupRecord, popupsGroup);
            }

            // Lưu thay đổi
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            Debug.Log(
                $"Successfully setup {this.popupRecords.Count} popup records as Addressable assets in 'Popups' group.");
        }

        private AddressableAssetGroup FindOrCreatePopupsGroup(AddressableAssetSettings settings)
        {
            // Tìm group "Popups" hiện có
            var existingGroup = settings.groups.FirstOrDefault(g => g.name == "Popups");
            if (existingGroup != null)
            {
                return existingGroup;
            }

            // Tạo group mới nếu không tìm thấy
            var newGroup = settings.CreateGroup("Popups", false, false, true, null, typeof(ContentCatalogData));
            Debug.Log("Created new Addressable group: 'Popups'");
            return newGroup;
        }

        private void SetupPopupAsAddressable(PopupRecord popupRecord, AddressableAssetGroup group)
        {
            var prefabPath = AssetDatabase.GetAssetPath(popupRecord.popupPrefab);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning($"Could not get asset path for prefab: {popupRecord.popupPrefab.name}");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            // Sử dụng CreateOrMoveEntry để tạo hoặc di chuyển asset entry
            var entry = addressableSettings.CreateOrMoveEntry(guid, group);
            if (entry != null)
            {
                // Đơn giản hóa địa chỉ
                this.SimplifyAddress(entry, popupRecord);
            }
            else
            {
                Debug.LogError($"Failed to create or move entry for prefab: {popupRecord.popupPrefab.name}");
            }
        }

        private void SimplifyAddress(AddressableAssetEntry entry, PopupRecord popupRecord)
        {
            // Đơn giản hóa địa chỉ bằng cách sử dụng tên prefab
            var simplifiedAddress = popupRecord.popupPrefab.name;
            entry.address = simplifiedAddress;

            Debug.Log($"Set Addressable address for '{popupRecord.popupPrefab.name}' to: {simplifiedAddress}");
        }
#endif
    }
}
