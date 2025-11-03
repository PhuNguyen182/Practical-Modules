using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Pool;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Cysharp.Threading.Tasks;
using PracticalModules.TypeCreator.Core;
using UnityEngine;
using ZLinq;

namespace Foundations.DataFlow.MasterDataController
{
    public class DynamicCustomDataManager : IDynamicCustomDataManager
    {
        private bool _isDisposed;
        private readonly Dictionary<Type, IDynamicGameDataHandler> _dynamicDataHandlers = new();

        public async UniTask InitializeDataHandlers(IMainDataManager mainDataManager)
        {
            Type interfaceType = typeof(IDynamicGameDataHandler);
            var allDataHandlerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .AsValueEnumerable()
                .SelectMany(GetTypesOfAssembly)
                .Where(type => interfaceType.IsAssignableFrom(type)
                               && (type.IsClass || type.IsValueType)
                               && !type.IsAbstract);
            
            foreach (Type dataHandlerType in allDataHandlerTypes)
            {
                if (TypeFactory.Create(dataHandlerType) is not IDynamicGameDataHandler dataHandler)
                    continue;
                
                await dataHandler.Load();
                dataHandler.InjectDataManager(mainDataManager);
                dataHandler.Initialize();
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
            where TDataHandler : class, IDynamicGameDataHandler
        {
            Type sourceDataType = typeof(TDataHandler);
            return _dynamicDataHandlers.GetValueOrDefault(sourceDataType) as TDataHandler;
        }
        
        public void DeleteSingleData(Type dataType) => _dynamicDataHandlers.GetValueOrDefault(dataType)?.Delete();
        
        public void DeleteAllData()
        {
            foreach (IDynamicGameDataHandler dynamicDataHandler in _dynamicDataHandlers.Values)
                dynamicDataHandler.Delete();
        }
        
        public async UniTask SaveAllDataAsync()
        {
            using var listPool = ListPool<UniTask>.Get(out List<UniTask> saveDataTasks);
            foreach (IDynamicGameDataHandler dynamicDataHandler in _dynamicDataHandlers.Values)
            {
                UniTask saveDataTask = dynamicDataHandler.SaveAsync();
                saveDataTasks.Add(saveDataTask);
            }
            
            await UniTask.WhenAll(saveDataTasks);
        }

        public void SaveAllData()
        {
            foreach (IDynamicGameDataHandler dynamicDataHandler in _dynamicDataHandlers.Values)
                dynamicDataHandler.Save();
            PlayerPrefs.Save();
        }

        ~DynamicCustomDataManager() => Dispose(false);

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
            {
                SaveAllData();
                _dynamicDataHandlers.Clear();
            }

            _isDisposed = true;
        }
    }
}
