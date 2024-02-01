using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    //
    // Summary:
    //     Resolves Services from the Container
    public interface IContainerProvider
    {
        //
        // Summary:
        //     Gets the Current Scope
       // IScopedProvider CurrentScope { get; }

        //
        // Summary:
        //     Resolves a given System.Type
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        // Returns:
        //     The resolved Service System.Type
        object Resolve(Type type);

        object Resolve(Type type, params (Type Type, object Instance)[] parameters);

        //
        // Summary:
        //     Resolves a given System.Type
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   name:
        //     The service name/key used when registering the System.Type
        //
        // Returns:
        //     The resolved Service System.Type
        object Resolve(Type type, string name);

        object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters);

        //
        // Summary:
        //     Creates a new scope
       // IScopedProvider CreateScope();
    }
    //
    // Summary:
    //     The registering container
    public interface IContainerRegistry
    {
        //
        // Summary:
        //     Registers an instance of a given System.Type
        //
        // Parameters:
        //   type:
        //     The service System.Type that is being registered
        //
        //   instance:
        //     The instance of the service or System.Type
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterInstance(Type type, object instance);

        //
        // Summary:
        //     Registers an instance of a given System.Type with the specified name or key
        //
        // Parameters:
        //   type:
        //     The service System.Type that is being registered
        //
        //   instance:
        //     The instance of the service or System.Type
        //
        //   name:
        //     The name or key to register the service
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterInstance(Type type, object instance, string name);

        //
        // Summary:
        //     Registers a Singleton with the given service and mapping to the specified implementation
        //     System.Type.
        //
        // Parameters:
        //   from:
        //     The service System.Type
        //
        //   to:
        //     The implementation System.Type
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterSingleton(Type from, Type to);

        //
        // Summary:
        //     Registers a Singleton with the given service and mapping to the specified implementation
        //     System.Type.
        //
        // Parameters:
        //   from:
        //     The service System.Type
        //
        //   to:
        //     The implementation System.Type
        //
        //   name:
        //     The name or key to register the service
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterSingleton(Type from, Type to, string name);

        //
        // Summary:
        //     Registers a Singleton with the given service System.Type factory delegate method.
        //
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   factoryMethod:
        //     The delegate method.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod);

        //
        // Summary:
        //     Registers a Singleton with the given service System.Type factory delegate method.
        //
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   factoryMethod:
        //     The delegate method using Prism.Ioc.IContainerProvider.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod);

        //
        // Summary:
        //     Registers a Singleton Service which implements service interfaces
        //
        // Parameters:
        //   type:
        //     The implementation System.Type.
        //
        //   serviceTypes:
        //     The service System.Type's.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        //
        // Remarks:
        //     Registers all interfaces if none are specified.
        IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes);

        //
        // Summary:
        //     Registers a Transient with the given service and mapping to the specified implementation
        //     System.Type.
        //
        // Parameters:
        //   from:
        //     The service System.Type
        //
        //   to:
        //     The implementation System.Type
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry Register(Type from, Type to);

        //
        // Summary:
        //     Registers a Transient with the given service and mapping to the specified implementation
        //     System.Type.
        //
        // Parameters:
        //   from:
        //     The service System.Type
        //
        //   to:
        //     The implementation System.Type
        //
        //   name:
        //     The name or key to register the service
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry Register(Type from, Type to, string name);

        //
        // Summary:
        //     Registers a Transient Service using a delegate method
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   factoryMethod:
        //     The delegate method.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry Register(Type type, Func<object> factoryMethod);

        //
        // Summary:
        //     Registers a Transient Service using a delegate method
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   factoryMethod:
        //     The delegate method using Prism.Ioc.IContainerProvider.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod);

        //
        // Summary:
        //     Registers a Transient Service which implements service interfaces
        //
        // Parameters:
        //   type:
        //     The implementing System.Type.
        //
        //   serviceTypes:
        //     The service System.Type's.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        //
        // Remarks:
        //     Registers all interfaces if none are specified.
        IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes);

        //
        // Summary:
        //     Registers a scoped service
        //
        // Parameters:
        //   from:
        //     The service System.Type
        //
        //   to:
        //     The implementation System.Type
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterScoped(Type from, Type to);

        //
        // Summary:
        //     Registers a scoped service using a delegate method.
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   factoryMethod:
        //     The delegate method.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod);

        //
        // Summary:
        //     Registers a scoped service using a delegate method.
        //
        // Parameters:
        //   type:
        //     The service System.Type.
        //
        //   factoryMethod:
        //     The delegate method.
        //
        // Returns:
        //     The Prism.Ioc.IContainerRegistry instance
        IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod);

        //
        // Summary:
        //     Determines if a given service is registered
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        // Returns:
        //     true if the service is registered.
        bool IsRegistered(Type type);

        //
        // Summary:
        //     Determines if a given service is registered with the specified name
        //
        // Parameters:
        //   type:
        //     The service System.Type
        //
        //   name:
        //     The service name or key used
        //
        // Returns:
        //     true if the service is registered.
        bool IsRegistered(Type type, string name);
    }
    //
    // Summary:
    //     Defines the contract for the modules deployed in the application.
    public interface IModule
    {
        //
        // Summary:
        //     Used to register types with the container that will be used by your application.
        void RegisterTypes(IContainerRegistry containerRegistry);

        //
        // Summary:
        //     Notifies the module that it has been initialized.
        void OnInitialized(IContainerProvider containerProvider);
    }
    //
    // Summary:
    //     This is the expected catalog definition for the ModuleManager. The ModuleCatalog
    //     holds information about the modules that can be used by the application. Each
    //     module is described in a ModuleInfo class, that records the name, type and location
    //     of the module.
    public interface IModuleCatalog
    {
        //
        // Summary:
        //     Gets all the Prism.Modularity.IModuleInfo classes that are in the Prism.Modularity.IModuleCatalog.
        IEnumerable<IModuleInfo> Modules { get; }

        //
        // Summary:
        //     Return the list of Prism.Modularity.IModuleInfos that moduleInfo depends on.
        //
        //
        // Parameters:
        //   moduleInfo:
        //     The Prism.Modularity.IModuleInfo to get the
        //
        // Returns:
        //     An enumeration of Prism.Modularity.IModuleInfo that moduleInfo depends on.
        IEnumerable<IModuleInfo> GetDependentModules(IModuleInfo moduleInfo);

        //
        // Summary:
        //     Returns the collection of Prism.Modularity.IModuleInfos that contain both the
        //     Prism.Modularity.IModuleInfos in modules, but also all the modules they depend
        //     on.
        //
        // Parameters:
        //   modules:
        //     The modules to get the dependencies for.
        //
        // Returns:
        //     A collection of Prism.Modularity.IModuleInfo that contains both all Prism.Modularity.IModuleInfos
        //     in modules and also all the Prism.Modularity.IModuleInfo they depend on.
        IEnumerable<IModuleInfo> CompleteListWithDependencies(IEnumerable<IModuleInfo> modules);

        //
        // Summary:
        //     Initializes the catalog, which may load and validate the modules.
        void Initialize();

        //
        // Summary:
        //     Adds a Prism.Modularity.IModuleInfo to the Prism.Modularity.IModuleCatalog.
        //
        // Parameters:
        //   moduleInfo:
        //     The Prism.Modularity.IModuleInfo to add.
        //
        // Returns:
        //     The Prism.Modularity.IModuleCatalog for easily adding multiple modules.
        IModuleCatalog AddModule(IModuleInfo moduleInfo);
    }
    public enum InitializationMode
    {
        //
        // Summary:
        //     The module will be initialized when it is available on application start-up.
        WhenAvailable,
        //
        // Summary:
        //     The module will be initialized when requested, and not automatically on application
        //     start-up.
        OnDemand
    }
    public enum ModuleState
    {
        //
        // Summary:
        //     Initial state for Prism.Modularity.IModuleInfos. The Prism.Modularity.IModuleInfo
        //     is defined, but it has not been loaded, retrieved or initialized yet.
        NotStarted,
        //
        // Summary:
        //     The assembly that contains the type of the module is currently being loaded.
        //
        //
        // Remarks:
        //     Used in Wpf to load a module dynamically
        LoadingTypes,
        //
        // Summary:
        //     The assembly that holds the Module is present. This means the type of the Prism.Modularity.IModule
        //     can be instantiated and initialized.
        ReadyForInitialization,
        //
        // Summary:
        //     The module is currently Initializing, by the Prism.Modularity.IModuleInitializer
        Initializing,
        //
        // Summary:
        //     The module is initialized and ready to be used.
        Initialized
    }
    //
    // Summary:
    //     Marker interface that allows both Prism.Modularity.IModuleInfoGroups and Prism.Modularity.IModuleInfos
    //     to be added to the Prism.Modularity.IModuleCatalog from code and XAML.
    public interface IModuleCatalogItem
    {
    }
    //
    // Summary:
    //     Set of properties for each Module
    public interface IModuleInfo : IModuleCatalogItem
    {
        //
        // Summary:
        //     The module names this instance depends on.
        Collection<string> DependsOn { get; set; }

        //
        // Summary:
        //     Gets or Sets the Prism.Modularity.IModuleInfo.InitializationMode
        InitializationMode InitializationMode { get; set; }

        //
        // Summary:
        //     The name of the module
        string ModuleName { get; set; }

        //
        // Summary:
        //     The module's type
        string ModuleType { get; set; }

        //
        // Summary:
        //     A string ref is a location reference to load the module as it may not be already
        //     loaded in the Appdomain in some cases may need to be downloaded.
        //
        // Remarks:
        //     This is only used for WPF
        string Ref { get; set; }

        //
        // Summary:
        //     Gets or Sets the current Prism.Modularity.ModuleState
        ModuleState State { get; set; }
    }
    [Serializable]
    public class ModuleInfo : IModuleInfo, IModuleCatalogItem
    {
        //
        // Summary:
        //     Gets or sets the name of the module.
        //
        // Value:
        //     The name of the module.
        public string ModuleName { get; set; }

        //
        // Summary:
        //     Gets or sets the module System.Type's AssemblyQualifiedName.
        //
        // Value:
        //     The type of the module.
        public string ModuleType { get; set; }

        //
        // Summary:
        //     Gets or sets the list of modules that this module depends upon.
        //
        // Value:
        //     The list of modules that this module depends upon.
        public Collection<string> DependsOn { get; set; }

        //
        // Summary:
        //     Specifies on which stage the Module will be initialized.
        public InitializationMode InitializationMode { get; set; }

        //
        // Summary:
        //     Reference to the location of the module assembly. The following are examples
        //     of valid Prism.Modularity.ModuleInfo.Ref values: file://c:/MyProject/Modules/MyModule.dll
        //     for a loose DLL in WPF.
        public string Ref { get; set; }

        //
        // Summary:
        //     Gets or sets the state of the Prism.Modularity.ModuleInfo with regards to the
        //     module loading and initialization process.
        public ModuleState State { get; set; }

        //
        // Summary:
        //     Initializes a new empty instance of Prism.Modularity.ModuleInfo.
        public ModuleInfo()
            : this(null, null, new string[0])
        {
        }

        //
        // Summary:
        //     Initializes a new instance of Prism.Modularity.ModuleInfo.
        //
        // Parameters:
        //   name:
        //     The module's name.
        //
        //   type:
        //     The module System.Type's AssemblyQualifiedName.
        //
        //   dependsOn:
        //     The modules this instance depends on.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     An System.ArgumentNullException is thrown if dependsOn is null.
        public ModuleInfo(string name, string type, params string[] dependsOn)
        {
            if (dependsOn == null)
            {
                throw new ArgumentNullException("dependsOn");
            }

            ModuleName = name;
            ModuleType = type;
            DependsOn = new Collection<string>();
            foreach (string item in dependsOn)
            {
                DependsOn.Add(item);
            }
        }

        //
        // Summary:
        //     Initializes a new instance of Prism.Modularity.ModuleInfo.
        //
        // Parameters:
        //   name:
        //     The module's name.
        //
        //   type:
        //     The module's type.
        public ModuleInfo(string name, string type)
            : this(name, type, new string[0])
        {
        }

        //
        // Summary:
        //     Initializes a new instance of Prism.Modularity.ModuleInfo.
        //
        // Parameters:
        //   moduleType:
        //     The module's type.
        public ModuleInfo(Type moduleType)
            : this(moduleType, moduleType.Name)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of Prism.Modularity.ModuleInfo.
        //
        // Parameters:
        //   moduleType:
        //     The module's type.
        //
        //   moduleName:
        //     The module's name.
        public ModuleInfo(Type moduleType, string moduleName)
            : this(moduleType, moduleName, InitializationMode.WhenAvailable)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of Prism.Modularity.ModuleInfo.
        //
        // Parameters:
        //   moduleType:
        //     The module's type.
        //
        //   moduleName:
        //     The module's name.
        //
        //   initializationMode:
        //     The module's Prism.Modularity.ModuleInfo.InitializationMode.
        public ModuleInfo(Type moduleType, string moduleName, InitializationMode initializationMode)
            : this(moduleName, moduleType.AssemblyQualifiedName)
        {
            InitializationMode = initializationMode;
        }
    }
}
