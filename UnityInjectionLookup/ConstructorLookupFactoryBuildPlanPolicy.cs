using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace UnityInjectionLookup
{
    class ConstructorLookupFactoryBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly IReadOnlyCollection<Type> parameterTypes;
        private readonly Type implementationType;

        public ConstructorLookupFactoryBuildPlanPolicy(IReadOnlyCollection<Type> parameterTypes, Type implementationType)
        {
            Guard.ArgumentNotNull(parameterTypes, "parameterTypes");
            Guard.ArgumentNotNull(implementationType, "implementationType");

            this.parameterTypes = parameterTypes;
            this.implementationType = implementationType;
            FactoryType = FunctionHelper.GetFuncType(parameterTypes.Count + 1)
                .MakeGenericType(parameterTypes.Concat(new[] { implementationType }).ToArray());
        }

        public Type FactoryType { get; private set; }

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var container = context.NewBuildUp<IUnityContainer>();
                string name = context.BuildKey.Name;
                var datas = context.GetBuildUpDatas(implementationType, name, parameterTypes);

                context.BuildKey = new NamedTypeBuildKey(FactoryType, name);
                context.Existing = FunctionHelper.MakeTyped(parameters => Resolve(container, datas, parameters),
                    parameterTypes, implementationType);
                context.BuildComplete = true;
            }
        }

        static object Resolve(IUnityContainer container, Dictionary<Type[], BuildUpData> datas, object[] arguments)
        {
            var parameterTypes = arguments.Select(argument => argument.GetType()).ToArray();
            BuildUpData data;

            if (!datas.TryGetValue(parameterTypes, out data))
                throw new ArgumentException("No registrations for following parameter types - " +
                    string.Join(", ", parameterTypes.Select(parameterType => parameterType.FullName)));

            return container.Resolve(data.BuildKey.Type, data.BuildKey.Name, data.GetOverrides(arguments));
        }
    }
}
