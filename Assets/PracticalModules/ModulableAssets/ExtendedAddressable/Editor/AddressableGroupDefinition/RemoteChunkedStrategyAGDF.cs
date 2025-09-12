#if USE_EXTENDED_ADDRESSABLE
using UnityEngine;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Editor.AddressableGroupDefinition
{
    [CreateAssetMenu(fileName = "RemoteChunkedStrategyAGDF",
        menuName = "ExtendedAddressable/Group Definition/Remote AGDF/RemoteChunkedStrategyAGDF")]
    public class RemoteChunkedStrategyAGDF : AddressableGroupDefinitionFile
    {
        public override BuildAndLoadMode BuildAndLoadMode => BuildAndLoadMode.Remote;
    }

}
#endif
