using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace UnityInjectionLookup
{
    class BuildUpData
    {
        private readonly IList<string> parameterNames;

        public BuildUpData(NamedTypeBuildKey buildKey, IList<string> parameterNames)
        {
            BuildKey = buildKey;
            this.parameterNames = parameterNames;
        }

        public NamedTypeBuildKey BuildKey { get; private set; }

        public ResolverOverride[] GetOverrides(IList<object> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (arguments.Count != parameterNames.Count)
                throw new ArgumentException("arguments");

            return parameterNames
                .Zip(arguments, (name, value) =>
                    new ParameterOverride(name, value))
                .ToArray<ResolverOverride>();
        }
    }
}
