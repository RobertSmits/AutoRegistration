using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Unity.AutoRegistration
{
    /// <summary>
    /// Registration options contract describes parameters of type registration operation
    /// </summary>
    public interface IRegistrationOptions
    {
        /// <summary>
        /// Gets or sets service lifetime to use to register type(s).
        /// </summary>
        /// <value>Service lifetime.</value>
        ServiceLifetime ServiceLifetime { get; set; }

        /// <summary>
        /// Gets or sets interfaces to register type(s) as.
        /// </summary>
        /// <value>Interfaces.</value>
        IEnumerable<Type> Interfaces { get; set; }

        /// <summary>
        /// Sets type being registered.
        /// </summary>
        /// <value>Target type.</value>
        Type Type { set; }
    }
}