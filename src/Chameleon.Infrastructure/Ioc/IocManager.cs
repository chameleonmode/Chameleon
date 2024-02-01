using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;

namespace Chameleon.Infrastructure.Ioc
{
    public class IocManager : IIocManager
    {
        private readonly IUnityContainer _container;
        private readonly IContainerRegistry _containerRegistry;
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        public IocManager(
            IContainerRegistry containerRegistry
            )
        {
            _containerRegistry = containerRegistry;
            _container = _containerRegistry.GetContainer();
        }

        public void RegisterTypes(Assembly assembly)
        {
            if (_assemblies.Contains(assembly))
            {
                return;
            }
            _assemblies.Add(assembly);

            RegisterTypes(
                GetTypesToRegister(assembly)
                );
        }

        private Type[] GetTypesToRegister(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => type.IsClass && typeof(IDependency).IsAssignableFrom(type))
                .ToArray();
        }

        private void RegisterTypes(Type[] types)
        {
            RegisterSingletons(types);
            RegisterTransients(types);
            RegisterScoped(types);
        }

        private void RegisterSingletons(Type[] types)
        {
            Register<ISingletonDependency>(types, (fromType, toType) => 
                _containerRegistry.RegisterSingleton(fromType, toType));
        }

        private void RegisterTransients(Type[] types)
        {
            Register<ITransientDependency>(types, (fromType, toType) =>
                _containerRegistry.Register(fromType, toType));
        }

        private void RegisterScoped(Type[] types)
        {
            Register<IScopedDependency>(types, (fromType, toType) =>
                _containerRegistry.RegisterScoped(fromType, toType));
        }

        private void Register<TInterface>(Type[] types, Action<Type, Type> register)
            where TInterface : IDependency
        {
            var objectType = typeof(object);
            var viewControlType = typeof(IUserControl);
            var typesToRegister = GetTypesToRegister<TInterface>(types);
            foreach (var typeToRegister in typesToRegister)
            {
                if (typeof(ISkipIOCRegistration).IsAssignableFrom(typeToRegister))
                {
                    continue;
                }

                var interfaceTypes = GetInterfaceTypesToRegister(typeToRegister);
                foreach (var interfaceType in interfaceTypes)
                {
                    //if (typeToRegister.Name != interfaceType.Name.Substring(1))
                    //{
                    //    continue;
                    //}

                    if (!_containerRegistry.IsRegistered(interfaceType))
                    {
                        register(interfaceType, typeToRegister);
                    }

                    if (viewControlType.IsAssignableFrom(interfaceType) && viewControlType != interfaceType)
                    {
                        var dependencyName = interfaceType.GetDependencyName();
                        // https://github.com/PrismLibrary/Prism/blob/master/src/Wpf/Prism.Wpf/Services/Dialogs/DialogService.cs
                        if (!_containerRegistry.IsRegistered(objectType, dependencyName))
                        {
                            // NOTE: that when using factory, we can not resolve instance with arguments
                            Func<IUnityContainer, object> factory
                                = c => c.Resolve(interfaceType);

                            _container.RegisterFactory(objectType, dependencyName, factory);
                        }
                    }
                }
            }
        }

        private Type[] GetTypesToRegister<TInterface>(Type[] types)
            where TInterface : IDependency
        {
            return types
                .Where(type => typeof(TInterface).IsAssignableFrom(type))
                .Where(type => !_containerRegistry.IsRegistered(type))
                .ToArray();
        }

        private Type[] GetInterfaceTypesToRegister(Type self)
        {
            return self.GetInterfaces()
                .Where(interfaceType => IsInterfaceTypeToRegister(interfaceType))
                .ToArray();
        }

        private bool IsInterfaceTypeToRegister(Type type)
        {
            if (IsInterfaceMarker(type))
            {
                return false;
            }

            if (IsInterfaceTypeToIgnore(type))
            {
                return false;
            }

            return true;
        }

        private bool IsInterfaceMarker(Type type)
        {
            return typeof(IDependency) == type
                || typeof(ISingletonDependency) == type
                || typeof(ITransientDependency) == type
                || typeof(IScopedDependency) == type;
        }

        private List<string> TypeNamePrefixToIgnore = new List<string>
        {
            "System",
            "Micosoft",
            "Windows",
            "Prism",
            "Presentation",
            "netstandard",
            "mscorlib",
            "Unity"
        };

        public object StringComparsion { get; private set; }

        private bool IsInterfaceTypeToIgnore(Type type)
        {
            return TypeNamePrefixToIgnore.Any(prefix =>
            {
                if (string.IsNullOrEmpty(type.FullName))
                {
                    return false;
                }

                return type.FullName.StartsWith(prefix, 
                    StringComparison.InvariantCultureIgnoreCase
                    );
            });
        }
    }
}
