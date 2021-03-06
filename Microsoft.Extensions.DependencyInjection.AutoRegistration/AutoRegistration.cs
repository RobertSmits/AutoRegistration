﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{
    /// <summary>
    /// Auto Registration extends popular Unity IoC container 
    /// and provides nice fluent syntax to configure rules for automatic types registration
    /// </summary>
    public class AutoRegistration : IAutoRegistration
    {
        private readonly List<RegistrationEntry> _registrationEntries = new List<RegistrationEntry>();

        private readonly List<Predicate<Assembly>> _excludedAssemblyFilters = new List<Predicate<Assembly>>();
        private readonly List<Predicate<Type>> _excludedTypeFilters = new List<Predicate<Type>>();

        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRegistration"/> class.
        /// </summary>
        /// <param name="services">Service container.</param>
        public AutoRegistration(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            _services = services;
        }

        /// <summary>
        /// Adds rule to include certain types that satisfy specified type filter
        /// and register them using specified registrator function
        /// </summary>
        /// <param name="typeFilter">Type filter.</param>
        /// <param name="registrator">Registrator function.</param>
        /// <returns>Auto registration</returns>
        public virtual IAutoRegistration Include(
            Predicate<Type> typeFilter,
            Action<Type, IServiceCollection> registrator)
        {
            if (typeFilter == null)
                throw new ArgumentNullException("typeFilter");
            if (registrator == null)
                throw new ArgumentNullException("registrator");

            _registrationEntries.Add(new RegistrationEntry(typeFilter, registrator, _services));
            return this;
        }

        /// <summary>
        /// Adds rule to include certain types that satisfy specified type filter
        /// and register them using specified registration options
        /// </summary>
        /// <param name="typeFilter">Type filter.</param>
        /// <param name="registrationOptions">RegistrationOptions options.</param>
        /// <returns>Auto registration</returns>
        public virtual IAutoRegistration Include(
            Predicate<Type> typeFilter,
            IRegistrationOptions registrationOptions)
        {
            if (typeFilter == null)
                throw new ArgumentNullException("typeFilter");
            if (registrationOptions == null)
                throw new ArgumentNullException("registrationOptions");

            _registrationEntries.Add(new RegistrationEntry(
                                         typeFilter,
                                         (t, c) =>
                                             {
                                                 registrationOptions.Type = t;
                                                 foreach (var contract in registrationOptions.Interfaces)
                                                 {
                                                     c.Add(new ServiceDescriptor(
                                                         contract,
                                                         t,
                                                         registrationOptions.ServiceLifetime));
                                                 }
                                             },
                                         _services));
            return this;
        }

        /// <summary>
        /// Adds rule to exclude certain assemblies that satisfy specified assembly filter
        /// and not consider their types
        /// </summary>
        /// <param name="filter">Type filter.</param>
        /// <returns>Auto registration</returns>
        public virtual IAutoRegistration ExcludeAssemblies(Predicate<Assembly> filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            _excludedAssemblyFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Adds rule to exclude certain types that satisfy specified type filter and not register them
        /// </summary>
        /// <param name="filter">Type filter.</param>
        /// <returns>Auto registration</returns>
        public virtual IAutoRegistration Exclude(Predicate<Type> filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            _excludedTypeFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Applies auto registration - scans loaded assemblies,
        /// check specified rules and register types that satisfy these rules
        /// </summary>
        public virtual void ApplyAutoRegistration()
        {
            if (_registrationEntries.Any())
                foreach (var type in AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !_excludedAssemblyFilters.Any(f => f(a)))
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !_excludedTypeFilters.Any(f => f(t)))
                    .Where(t => !t.IsAbstract))
                    foreach (var entry in _registrationEntries)
                        entry.RegisterIfSatisfiesFilter(type);
        }

        private class RegistrationEntry
        {
            private readonly Predicate<Type> _typeFilter;
            private readonly Action<Type, IServiceCollection> _registrator;
            private readonly IServiceCollection _services;

            public RegistrationEntry(Predicate<Type> typeFilter,
                                     Action<Type, IServiceCollection> registrator, IServiceCollection services)
            {
                _typeFilter = typeFilter;
                _registrator = registrator;
                _services = services;
            }

            public void RegisterIfSatisfiesFilter(Type type)
            {
                if (_typeFilter(type))
                    _registrator(type, _services);
            }
        }
    }
}