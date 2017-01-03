using System;
using System.Collections.Generic;
using Autofac;

namespace FWF.KeyExchange
{
    public static class ComponentContextExtensions
    {
        public static IEnumerable<T> ResolveAll<T>(this IComponentContext componentContext)
        {
            //
            Type genericType = typeof(IEnumerable<>).MakeGenericType(typeof(T));

            return componentContext.Resolve(genericType) as IEnumerable<T>;
        }

    }

}
