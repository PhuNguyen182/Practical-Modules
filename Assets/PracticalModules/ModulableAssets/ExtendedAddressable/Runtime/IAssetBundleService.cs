#if USE_EXTENDED_ADDRESSABLE
using System;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using Cysharp.Threading.Tasks;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime
{
    public interface IAssetBundleService : IDisposable
    {
        public IAssetBundleLoader AssetBundleLoader { get; }
        public IAssetBundleCleaner AssetBundleCleaner { get; }
        public IAssetBundleUpdater AssetBundleUpdater { get; }
        public IAssetBundleResourceLocator AssetBundleResourceLocator { get; }
        public IAssetBundleDownloader AssetBundleDownloader { get; }

        public UniTask<bool> Initialize(Action onInitializationComplete = null,
            Action onInitializationFailed = null);
        
        public UniTask<bool> InitializeAsync(UniTask onInitializationComplete,
            UniTask onInitializationFailed);
    }
}
#endif