using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityInjectionLookup
{
    internal class TypeArrayEqualityComparer : IEqualityComparer<Type[]>
    {
        public bool Equals(Type[] x, Type[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(Type[] obj)
        {
            return obj.Take(5).Aggregate(0, (seed, type) => (seed * 397) ^ type.GetHashCode());
        }
    }
}
