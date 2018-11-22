using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Registration;

namespace UnityInjectionLookup
{
    internal static class BuilderContextExtensions
    {
        public static IList<object> ResolveWithOverrides(this IBuilderContext context, IEnumerable<Type> typesToResolve) =>
            typesToResolve.Select(context.ResolveWithOverride).ToList();

        public static object ResolveWithOverride(this IBuilderContext context, Type typeToResolve) =>
            context
                .GetOverriddenResolver(typeToResolve)
                ?.Resolve(context)
                ?? ((IUnityContainer)context
                    .NewBuildUp(typeof(IUnityContainer), null))
                    .Resolve(typeToResolve);

        public static BuildUpData GetBuildUpData(this IBuilderContext context, Type registeredType, string name, Type[] argumentTypes)
        {
            var registrationPair = context.GetRegistrations(registeredType, name)
                    .Select(registration => new
                    {
                        registration.Name,
                        Constructor = registration.MappedToType
                            .GetConstructor(argumentTypes)
                    })
                    .Single(v => v.Constructor != null);

            return new BuildUpData(
                new NamedTypeBuildKey(registeredType, registrationPair.Name),
                registrationPair.Constructor
                    .GetParameters()
                    .Select(parameter => parameter.Name)
                    .ToList());
        }

        public static Dictionary<Type[], BuildUpData> GetBuildUpDatas(this IBuilderContext context,
            Type registeredType, string name, IReadOnlyCollection<Type> argumentTypes)
        {
            return context.GetRegistrations(registeredType, name)
                .SelectMany(registration => registration.MappedToType
                    .GetConstructors()
                    .Select(c => c.GetParameters())
                    .Where(parameters => parameters.Length == argumentTypes.Count &&
                        parameters
                            .Zip(argumentTypes, (p, type) => type.IsAssignableFrom(p.ParameterType))
                            .All(b => b))
                    .Select(parameters =>
                        new
                        {
                            Types = parameters.Select(p => p.ParameterType).ToArray(),
                            Data = new BuildUpData(
                                new NamedTypeBuildKey(registeredType, registration.Name),
                                parameters
                                .Select(parameter => parameter.Name)
                                .ToList())
                        }))
                .ToDictionary(x => x.Types, x => x.Data, new TypeArrayEqualityComparer());
        }

        public static IEnumerable<IContainerRegistration> GetRegistrations(this IBuilderContext context,
            Type registeredType, string name) =>
            ((IUnityContainer)context
                    .NewBuildUp(typeof(IUnityContainer), null)).Registrations
                .Where(registration => registration.RegisteredType == registeredType &&
                                       !(registration.LifetimeManager is ContainerControlledLifetimeManager) &&
                                       registration.Name != name);
    }
}
