using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{
    /// <summary>
    /// Extension methods to various types
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Gets type attribute.
        /// </summary>
        /// <typeparam name="TAttr">Type of the attribute.</typeparam>
        /// <param name="type">Target type.</param>
        /// <returns>Attribute value</returns>
        public static TAttr GetAttribute<TAttr>(this Type type)
            where TAttr : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes(false).Single(a => typeof(TAttr) == a.GetType()) as TAttr;
        }

        /// <summary>
        /// Configures auto registration - starts chain of fluent configuration
        /// </summary>
        /// <param name="services">Service container.</param>
        /// <returns>Auto registration</returns>
        public static IServiceCollection AddAutoRegistration(this IServiceCollection services, Func<IAutoRegistration, IAutoRegistration> autoRegisterFunc)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            //return new AutoRegistration(services);
            var autoRegistration = new AutoRegistration(services);
            autoRegisterFunc(autoRegistration)
                .ApplyAutoRegistration();

            return services;
        }

        /// <summary>
        /// Configures auto registration - starts chain of fluent configuration
        /// </summary>
        /// <param name="services">Service container.</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration ConfigureAutoRegistration(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            return new AutoRegistration(services);
        }

        /// <summary>
        /// Adds rule to exclude certain assemblies (that name starts with System or mscorlib) 
        /// and not consider their types
        /// </summary>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration ExcludeSystemAssemblies(this IAutoRegistration autoRegistration)
        {
            autoRegistration.ExcludeAssemblies(a => a.GetName().FullName.StartsWith("System.")
                || a.GetName().FullName.StartsWith("mscorlib")
                || a.GetName().Name.Equals("System"));
            return autoRegistration;
        }
    }
}