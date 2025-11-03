using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Foundations.DataFlow.MicroData.StaticDataControllers;
using Cysharp.Threading.Tasks;
using PracticalModules.TypeCreator.Core;
using ZLinq;

namespace Foundations.DataFlow.MasterDataController
{
    public class StaticCustomDataManager : IStaticCustomDataManager
    {
        private bool _isDisposed;
        private readonly Dictionary<Type, IStaticGameDataHandler> _dynamicDataHandlers = new();
        
        public async UniTask InitializeDataHandlers(IMainDataManager mainDataManager)
        {
            Type interfaceType = typeof(IStaticGameDataHandler);
            var allDataHandlerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .AsValueEnumerable()
                .SelectMany(GetTypesOfAssembly)
                .Where(type => interfaceType.IsAssignableFrom(type)
                               && (type.IsClass || type.IsValueType)
                               && !type.IsAbstract);
            
            foreach (Type dataHandlerType in allDataHandlerTypes)
            {
                if (TypeFactory.Create(dataHandlerType) is not IStaticGameDataHandler dataHandler)
                    continue;
                
                await dataHandler.Initialize();
                _dynamicDataHandlers.Add(dataHandlerType, dataHandler);
            }

            IEnumerable<Type> GetTypesOfAssembly(Assembly assembly)
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    Debug.LogError($"ReflectionTypeLoadException: {e.Message}");
                    return e.Types.Where(typeLoad => typeLoad != null);
                }
            }
        }

        public TDataHandler GetDataHandler<TDataHandler>()
            where TDataHandler : class, IStaticGameDataHandler
        {
            Type sourceDataType = typeof(TDataHandler);
            return _dynamicDataHandlers.GetValueOrDefault(sourceDataType) as TDataHandler;
        }
        
        ~StaticCustomDataManager() => Dispose(false);
        
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;
            
            if (disposing)
                _dynamicDataHandlers.Clear();
            
            _isDisposed = true;
        }
    }
}
