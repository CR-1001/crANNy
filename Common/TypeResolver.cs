/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    public static class TypeResolver
    {
        private static Dictionary<Type, List<Type>> _CacheTypesInstanceableAssignableTo
            = new Dictionary<Type, List<Type>>();


        public static IEnumerable<Type> GetTypesInstanceableAssignableTo<T>()
        {
            return GetTypesInstanceableAssignableTo(typeof(T));
        }

        public static IEnumerable<Type> GetTypesInstanceableAssignableTo(params Type[] types)
        {
            foreach(var type in types)
            {
                foreach(var typex in GetTypesInstanceableAssignableTo(type))
                {
                    yield return typex;
                }
            }
        }

        public static Type GetType(String typeFullName, String assemblyFullName)
        {
            var types = GetExportedTypesAssembliesAppDomain();

            var type = types.SingleOrDefault(
                t => t.FullName == typeFullName
                    && t.FullName == assemblyFullName);

            return type;
        }

        public static IEnumerable<Assembly> GetAssembliesAppDomain()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies;
        }

        public static IEnumerable<Type> GetExportedTypesAssembliesAppDomain()
        {
            var types
                = GetAssembliesAppDomain()
                .SelectMany(a => 
                {
                    try
                    {
                        return a.GetExportedTypes();
                    }
                    catch (Exception)
                    {
                        return new Type[0];
                    }
                    
                });

            return types;
        }

        public static IEnumerable<Type> GetTypesInstanceableAssignableTo(Type type)
        {
            lock (_CacheTypesInstanceableAssignableTo)
            {
                List<Type> types;

                if (!_CacheTypesInstanceableAssignableTo.TryGetValue(type, out types))
                {
                    types
                        = GetExportedTypesAssembliesAppDomain()
                        .Where(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                        .ToList();

                    _CacheTypesInstanceableAssignableTo[type] = types;
                }

                return types.AsEnumerable();
            }
        }

        public static IEnumerable<Type> GetTypesInstanceableAttributed<T>()
            where T : Attribute
        {
            var typesAll = GetTypesInstanceableAssignableTo(typeof(Object));

            var typesAttributed
                = typesAll
                .Where(t => t.GetCustomAttributes<T>().Any());

            return typesAttributed;
        }

        public static IEnumerable<Type> GetTypesTransmittable()
        {
            var types
                = TypeResolver.GetTypesInstanceableAssignableTo(
                    typeof(Object))
                .Where(t => t.IsClass
                    && !t.IsGenericType
                    && t.GetCustomAttributes<DataContractAttribute>().Any()
                    && t.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public,
                        null,
                        new Type[0],
                        new ParameterModifier[0]) != null);

            return types;
        }
    }
}
