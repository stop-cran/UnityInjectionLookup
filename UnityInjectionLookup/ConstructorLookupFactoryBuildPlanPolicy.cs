using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Builder;
using Unity.Policy;

namespace UnityInjectionLookup
{
    internal class ConstructorLookupFactoryBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly IReadOnlyCollection<Type> parameterTypes;
        private readonly Type implementationType;

        public ConstructorLookupFactoryBuildPlanPolicy(IReadOnlyCollection<Type> parameterTypes, Type implementationType)
        {
            this.parameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));
            this.implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            FactoryType = FunctionHelper.GetFuncType(parameterTypes.Count + 1)
                .MakeGenericType(parameterTypes.Concat(new[] { implementationType }).ToArray());
        }

        public Type FactoryType { get; }

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var container = (IUnityContainer)context.NewBuildUp(typeof(IUnityContainer), null);
                var name = context.BuildKey.Name;
                var datas = context.GetBuildUpDatas(implementationType, name, parameterTypes);

                context.BuildKey = new NamedTypeBuildKey(FactoryType, name);
                context.Existing = FunctionHelper.MakeTyped(parameters => Resolve(container, datas, parameters),
                    parameterTypes, implementationType);
                context.BuildComplete = true;
            }
        }

        private static object Resolve(IUnityContainer container, Dictionary<Type[], BuildUpData> datas, object[] arguments)
        {
            var parameterTypes = arguments.Select(argument => argument.GetType()).ToArray();

            if (!datas.TryGetValue(parameterTypes, out BuildUpData data))
                throw new ArgumentException("No registrations for following parameter types - " +
                    string.Join(", ", parameterTypes.Select(parameterType => parameterType.FullName)));

            return container.Resolve(data.BuildKey.Type, data.BuildKey.Name, data.GetOverrides(arguments));
        }
    }
}
