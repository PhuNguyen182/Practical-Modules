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
        private static readonly Type HandlerInterfaceType = typeof(IStaticGameDataHandler);
        
        private static readonly Func<Type, bool> IsNotNull = IsTypeNotNull;
        private static readonly Func<Assembly, IEnumerable<Type>> GetTypesDelegate = GetTypesOfAssembly;
        private static readonly Func<Type, bool> TypeIsConcrete = TypeIsConcreteClassOrStruct;
        private static readonly Func<Type, bool> TypeValidation = IsStaticDataHandler;
        
        public async UniTask InitializeDataHandlers(IMainDataManager mainDataManager)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allDataHandlerTypes = assemblies
                .AsValueEnumerable()
                .SelectMany(GetTypesDelegate)
                .Where(TypeIsConcrete)
                .Where(TypeValidation);
            
            foreach (Type dataHandlerType in allDataHandlerTypes)
            {
                if (TypeFactory.Create(dataHandlerType) is not IStaticGameDataHandler dataHandler)
                    continue;
                
                await dataHandler.Initialize();
                _dynamicDataHandlers.Add(dataHandlerType, dataHandler);
            }
        }
        
        private static bool IsTypeNotNull(Type type) => type != null;
        
        private static IEnumerable<Type> GetTypesOfAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Debug.LogError($"ReflectionTypeLoadException: {e.Message}");
                return e.Types.Where(IsNotNull);
            }
        }

        private static bool IsStaticDataHandler(Type type) => HandlerInterfaceType.IsAssignableFrom(type);
        
        private static bool TypeIsConcreteClassOrStruct(Type type)
            => (type.IsClass && !type.IsAbstract) || type.IsValueType;

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
