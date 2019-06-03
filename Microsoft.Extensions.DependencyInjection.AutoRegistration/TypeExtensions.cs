﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{
    public static class TypeExtensions
    {

        public static IEnumerable<Type> GetImplementedInterfacesFixed(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return GetImplementedInterfacesFixed(typeInfo);
        }

        public static IEnumerable<Type> GetImplementedInterfacesFixed(this TypeInfo typeInfo)
        {
            if (typeInfo.IsGenericTypeDefinition)
                return typeInfo.ImplementedInterfaces.Select(t => t.GetTypeInfo().Assembly.GetType(t.Namespace + "." + t.Name));
            else
                return typeInfo.ImplementedInterfaces;
        }

    }
}
