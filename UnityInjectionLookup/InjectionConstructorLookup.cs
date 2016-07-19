using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace UnityInjectionLookup
{
    public class InjectionConstructorLookup : InjectionMember
    {
        private readonly IReadOnlyCollection<Type> argumentTypes;

        public InjectionConstructorLookup(IEnumerable<Type> argumentTypes)
        {
            Guard.ArgumentNotNull(argumentTypes, "argumentTypes");
            this.argumentTypes = new ReadOnlyCollection<Type>(argumentTypes.ToList());
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var factoryPolicy = new ConstructorLookupFactoryBuildPlanPolicy(argumentTypes, implementationType);

            policies.Set<IBuildPlanPolicy>(new ConstructorLookupBuildPlanPolicy(argumentTypes),
                new NamedTypeBuildKey(implementationType, name));
            policies.Set<IBuildPlanPolicy>(factoryPolicy, new NamedTypeBuildKey(factoryPolicy.FactoryType, name));
        }
    }


    public class InjectionConstructorLookup<T> : InjectionConstructorLookup
    {
        public InjectionConstructorLookup() : base(new[] { typeof(T) }) { }
    }


    public class InjectionConstructorLookup<T1, T2> : InjectionConstructorLookup
    {
        public InjectionConstructorLookup() : base(new[] { typeof(T1), typeof(T2) }) { }
    }
}
