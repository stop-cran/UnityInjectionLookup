using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace UnityInjectionLookup
{
    class ConstructorLookupBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly IReadOnlyCollection<Type> argumentTypes;

        public ConstructorLookupBuildPlanPolicy(IReadOnlyCollection<Type> argumentTypes)
        {
            Guard.ArgumentNotNull(argumentTypes, "argumentTypes");
            this.argumentTypes = argumentTypes;
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
                context.Existing = context.NewBuildUp(data.BuildKey);
                context.BuildComplete = true;
            }
        }
    }
}
