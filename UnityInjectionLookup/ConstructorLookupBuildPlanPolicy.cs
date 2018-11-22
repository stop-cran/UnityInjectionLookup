using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Policy;

namespace UnityInjectionLookup
{
    internal class ConstructorLookupBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly IReadOnlyCollection<Type> argumentTypes;

        public ConstructorLookupBuildPlanPolicy(IReadOnlyCollection<Type> argumentTypes)
        {
            this.argumentTypes = argumentTypes ?? throw new ArgumentNullException(nameof(argumentTypes));
        }

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var arguments = context.ResolveWithOverrides(argumentTypes);
                var data = context.GetBuildUpData(context.BuildKey.Type, context.BuildKey.Name,
                    arguments.Select(argument => argument.GetType()).ToArray());

                context.AddResolverOverrides(data.GetOverrides(arguments));
                context.BuildKey = data.BuildKey;
                context.Existing = context.NewBuildUp(data.BuildKey.Type, data.BuildKey.Name);
                context.BuildComplete = true;
            }
        }
    }
}
