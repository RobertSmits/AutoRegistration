using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{
    /// <summary>
    /// Type registration options
    /// </summary>
    public class RegistrationOptions : IFluentRegistration
    {
        private Type _type;

        private Func<Type, IEnumerable<Type>> _interfacesToRegisterAsResolver = t => new List<Type>(t.GetImplementedInterfacesFixed());
        private Func<Type, ServiceLifetime> _serviceLifetimeToRegisterWithResolver = t => ServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets service lifetime to use to register type(s).
        /// </summary>
        /// <value>Service lifetime.</value>
        public ServiceLifetime ServiceLifetime
        {
            get
            {
                return _serviceLifetimeToRegisterWithResolver(_type);
            }
            set
            {
                _serviceLifetimeToRegisterWithResolver = t => value;
            }
        }

        /// <summary>
        /// Gets or sets interfaces to register type(s) as.
        /// </summary>
        /// <value>Interfaces.</value>
        public IEnumerable<Type> Interfaces
        {
            get
            {
                return _interfacesToRegisterAsResolver(_type);
            }
            set
            {
                _interfacesToRegisterAsResolver = t => value;
            }
        }

        /// <summary>
        /// Sets type being registered.
        /// </summary>
        /// <value>Target type.</value>
        public Type Type
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _type = value;
            }
        }

        /// <summary>
        /// Specifies service lifetime to use when registering type
        /// </summary>
        /// <param name="serviceLifetime">The type of the service lifetime.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingLifetime(ServiceLifetime serviceLifetime)
        {
            _serviceLifetimeToRegisterWithResolver = t => serviceLifetime;
            return this;
        }

        /// <summary>
        /// Specifies service lifetime resolver function, that by given type return service lifetime to use when registering type
        /// </summary>
        /// <param name="lifetimeResolver">Service lifetime resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingLifetime(Func<Type, ServiceLifetime> lifetimeResolver)
        {
            if (lifetimeResolver == null)
                throw new ArgumentNullException("lifetimeResolver");

            _serviceLifetimeToRegisterWithResolver = lifetimeResolver;
            return this;
        }

        /// <summary>
        /// Specifies Singleton service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingSingletonMode()
        {
            _serviceLifetimeToRegisterWithResolver = t => ServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Specifies Scoped service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingScopedMode()
        {
            _serviceLifetimeToRegisterWithResolver = t => ServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Specifies Transient service lifetime to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingTransientMode()
        {
            _serviceLifetimeToRegisterWithResolver = t => ServiceLifetime.Transient;
            return this;
        }

        /// <summary>
        /// Specifies interface to register type as
        /// </summary>
        /// <typeparam name="TContact">The type of the interface.</typeparam>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As<TContact>() where TContact : class
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { typeof(TContact) };
            return this;
        }

        /// <summary>
        /// Specifies interface resolver function that by given type returns interface register type as
        /// </summary>
        /// <param name="typeResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As(Func<Type, Type> typeResolver)
        {
            if (typeResolver == null)
                throw new ArgumentNullException("typeResolver");

            _interfacesToRegisterAsResolver = t => new List<Type> { typeResolver(t) };
            return this;
        }

        /// <summary>
        /// Specifies interface resolver function that by given type returns interfaces register type as
        /// </summary>
        /// <param name="typesResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As(Func<Type, Type[]> typesResolver)
        {
            if (typesResolver == null)
                throw new ArgumentNullException("typesResolver");

            _interfacesToRegisterAsResolver = t => new List<Type>(typesResolver(t));
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as its first interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsFirstInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetImplementedInterfacesFixed().First() };
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as its single interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsSingleInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetImplementedInterfacesFixed().Single() };
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as all its interfaces
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsAllInterfacesOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type>(t.GetImplementedInterfacesFixed());
            return this;
        }
    }
}