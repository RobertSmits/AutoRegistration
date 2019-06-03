using System;

namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{
    /// <summary>
    /// Fluent registration options contract describes number of operations 
    /// to fluently set registration option values
    /// </summary>
    public interface IFluentRegistration : IRegistrationOptions
    {

        /// <summary>
        /// Specifies service lifetime resolver function, that by given type return service lifetime to use when registering type
        /// </summary>
        /// <param name="lifetimeResolver">Service lifetime resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingLifetime(Func<Type, ServiceLifetime> serviceLifetime);

        /// <summary>
        /// Specifies service lifetime to use when registering type
        /// </summary>
        /// <typeparam name="serviceLifetime">The type of the service lifetime.</typeparam>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingLifetime(ServiceLifetime serviceLifetime);

        /// <summary>
        /// Specifies Singleton service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingSingletonMode();


        /// <summary>
        /// Specifies Scoped service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingScopedMode();

        /// <summary>
        /// Specifies Transient service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingTransientMode();

        IFluentRegistration As<TContact>() where TContact : class;

        /// <summary>
        /// Specifies interface or type resolver function that by given type returns type to register type as
        /// </summary>
        /// <param name="typeResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration As(Func<Type, Type> typeResolver);

        /// <summary>
        /// Specifies interface or type resolver function that by given type returns types to register type as
        /// </summary>
        /// <param name="typesResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration As(Func<Type, Type[]> typesResolver);

        /// <summary>
        /// Specifies that type should be registered as its first interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsFirstInterfaceOfType();

        /// <summary>
        /// Specifies that type should be registered as its single interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsSingleInterfaceOfType();

        /// <summary>
        /// Specifies that type should be registered as all its interfaces
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsAllInterfacesOfType();
    }
}