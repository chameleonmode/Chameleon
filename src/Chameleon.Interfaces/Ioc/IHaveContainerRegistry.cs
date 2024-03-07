namespace Chameleon.Interfaces.Ioc;

public interface IHaveContainerRegistry : ISingletonDependency
{
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
    void RegisterSingleton(Type from, Type to);

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
    void Register(Type from, Type to);

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
    void RegisterScoped(Type from, Type to);

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
    void ResolveAndRegister(Type interfaceType, Type objectType, string dependencyName);

    void RegisterInstance<TInterface>(TInterface instance);
    object Resolve(Type interfaceType);
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
    void Register(Type objectType, Type interfaceType, string dependencyName, Func<IHaveContainerRegistry, object> factoryMethod);
}
