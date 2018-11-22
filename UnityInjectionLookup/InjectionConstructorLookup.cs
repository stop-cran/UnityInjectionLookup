using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Builder;
using Unity.Policy;
using Unity.Registration;

namespace UnityInjectionLookup
{
    public class InjectionConstructorLookup : InjectionMember
    {
        private readonly IReadOnlyCollection<Type> argumentTypes;

        public InjectionConstructorLookup(IEnumerable<Type> argumentTypes)
        {
            this.argumentTypes = new ReadOnlyCollection<Type>((argumentTypes
                ?? throw new ArgumentNullException(nameof(argumentTypes))).ToList());
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var factoryPolicy = new ConstructorLookupFactoryBuildPlanPolicy(
                argumentTypes ?? throw new ArgumentNullException(nameof(argumentTypes)),
                implementationType ?? throw new ArgumentNullException(nameof(implementationType)));

            policies.Set<IBuildPlanPolicy>(new ConstructorLookupBuildPlanPolicy(argumentTypes),
                new NamedTypeBuildKey(implementationType, name));
            policies.Set<IBuildPlanPolicy>(factoryPolicy, new NamedTypeBuildKey(factoryPolicy.FactoryType, name));
        }
    }

    public class InjectionConstructorLookup<T> : InjectionConstructorLookup
    {
        public InjectionConstructorLookup() : base(new[] { typeof(T) })
        {
        }
    }

    public class InjectionConstructorLookup<T1, T2> : InjectionConstructorLookup
    {
        public InjectionConstructorLookup() : base(new[] { typeof(T1), typeof(T2) })
        {
        }
    }
}
