using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using PracticalModules.MessageBrokers.MessageTypes;
using MessagePipe;
using ZLinq;

namespace PracticalModules.MessageBrokers.Core
{
    public class MessageBrokerInitializer
    {
        private readonly BuiltinContainerBuilder _containerBuilder;

        public MessageBrokerInitializer()
        {
            _containerBuilder = new();
            _containerBuilder.AddMessagePipe();

            AddMessageBrokers();
            IServiceProvider serviceProvider = _containerBuilder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(serviceProvider);
        }

        private void AddMessageBrokers()
        {
            const string addMessageBrokerMethodName = "AddMessageBroker";

            Type interfaceType = typeof(IMessageType);
            Type[] allMessageTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .AsValueEnumerable()
                .SelectMany(GetTypesOfAssembly)
                .Where(type => interfaceType.IsAssignableFrom(type)
                               && (type.IsClass || type.IsValueType)
                               && !type.IsAbstract).ToArray();

            Type type = _containerBuilder.GetType();
            MethodInfo addMessageBrokerMethod = type.GetMethods().AsValueEnumerable()
                .FirstOrDefault(method =>
                    string.CompareOrdinal(method.Name, addMessageBrokerMethodName) == 0 &&
                    method.IsGenericMethodDefinition &&
                    method.GetGenericArguments().Length == 1);

            for (int i = 0; i < allMessageTypes.Length; i++)
            {
                Type messageType = allMessageTypes[i];
                MethodInfo genericMethod = addMessageBrokerMethod.MakeGenericMethod(messageType);
                genericMethod.Invoke(_containerBuilder, null);
            }
            
            IEnumerable<Type> GetTypesOfAssembly(Assembly assembly)
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(typeLoad => typeLoad != null);
                }
            }
        }
    }
}