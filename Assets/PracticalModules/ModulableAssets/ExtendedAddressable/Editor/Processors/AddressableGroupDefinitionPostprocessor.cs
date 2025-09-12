#if USE_EXTENDED_ADDRESSABLE
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using PracticalModules.ModulableAssets.ExtendedAddressable.Editor.AddressableGroupDefinition;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Editor.Processors
{
    public class AddressableGroupDefinitionPostprocessor : AssetPostprocessor
    {
        // Triggered after assets are imported/created
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return;

            bool anyChanged = false;

            foreach (string assetPath in importedAssets)
            {
                AddressableGroupDefinitionFile addressableGroupDefinitionFile =
                    AssetDatabase.LoadAssetAtPath<AddressableGroupDefinitionFile>(assetPath);
                if (addressableGroupDefinitionFile == null)
                    continue;

                if (string.IsNullOrEmpty(assetPath))
                    continue;
                
                string desiredGroupName = !string.IsNullOrEmpty(addressableGroupDefinitionFile.GroupName)
                    ? addressableGroupDefinitionFile.GroupName
                    : Path.GetFileNameWithoutExtension(assetPath);

                AddressableAssetGroup group = settings.FindGroup(desiredGroupName);
                if (group == null)
                {
                    if (addressableGroupDefinitionFile.template == null)
                        return;

                    AddressableAssetGroupTemplate groupTemplate = addressableGroupDefinitionFile.BuildAndLoadMode switch
                    {
                        BuildAndLoadMode.Remote => addressableGroupDefinitionFile.template.remoteTemplateGroup,
                        BuildAndLoadMode.Local => addressableGroupDefinitionFile.template.localTemplateGroup,
                        _ => null
                    };
                    
                    if (groupTemplate == null)
                        return;
                    
                    List<AddressableAssetGroupSchema> schemasCopy = new();
                    foreach (AddressableAssetGroupSchema schema in groupTemplate.SchemaObjects)
                    {
                        AddressableAssetGroupSchema schemaCopy = Object.Instantiate(schema);
                        schemaCopy.name = schema.name;
                        schemasCopy.Add(schemaCopy);
                    }

                    group = settings.CreateGroup(desiredGroupName, false, false, false, schemasCopy, typeof(BundledAssetGroupSchema));
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, group, true);
                    anyChanged = true;
                }

                // Persist the group name on the asset
                if (addressableGroupDefinitionFile.GroupName != group.Name)
                {
                    addressableGroupDefinitionFile.groupName = group.Name;
                    EditorUtility.SetDirty(addressableGroupDefinitionFile);
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif


