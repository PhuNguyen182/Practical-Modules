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
        private static readonly Func<Assembly, IEnumerable<Type>> GetTypesOfAssemblyFunc = GetTypesOfAssembly;
        private static readonly Func<Type, bool> TypeIsConcreteClassOrStructFunc = TypeIsConcreteClassOrStruct;
        private static readonly Func<Type, bool> TypeValidation = IsMessageBrokerData;
        private static readonly Type HandlerInterfaceType = typeof(IMessageType);

        public MessageBrokerInitializer()
        {
            this._containerBuilder = new BuiltinContainerBuilder();
            this._containerBuilder.AddMessagePipe();
            this.AddMessageBrokers();
            IServiceProvider serviceProvider = this._containerBuilder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(serviceProvider);
        }

        private void AddMessageBrokers()
        {
            const string addMessageBrokerMethodName = "AddMessageBroker";

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allMessageTypes = assemblies
                .AsValueEnumerable()
                .SelectMany(GetTypesOfAssemblyFunc)
                .Where(TypeIsConcreteClassOrStructFunc)
                .Where(TypeValidation);

            Type type = _containerBuilder.GetType();
            MethodInfo addMessageBrokerMethod = type.GetMethods(BindingFlags.Public)
                .AsValueEnumerable()
                .FirstOrDefault(method =>
                    string.CompareOrdinal(method.Name, addMessageBrokerMethodName) == 0 &&
                    method.IsGenericMethodDefinition &&
                    method.GetGenericArguments().Length == 1);

            if (addMessageBrokerMethod == null)
            {
                throw new InvalidOperationException(
                    "AddMessageBroker<T> method not found on BuiltinContainerBuilder");
            }

            foreach (Type messageType in allMessageTypes)
            {
                try
                {
                    MethodInfo genericMethod = addMessageBrokerMethod.MakeGenericMethod(messageType);
                    Action action =
                        (Action)Delegate.CreateDelegate(typeof(Action), this._containerBuilder, genericMethod);
                    action();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to register message broker for type {messageType.FullName}", ex);
                }
            }
        }

        private static IEnumerable<Type> GetTypesOfAssembly(Assembly assembly)
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

        private static bool TypeIsConcreteClassOrStruct(Type type)
            => (type.IsClass || type.IsValueType) && !type.IsAbstract &&
               type.GetCustomAttribute<MessageBrokerAttribute>() != null;

        private static bool IsMessageBrokerData(Type type) => HandlerInterfaceType.IsAssignableFrom(type);
    }
}