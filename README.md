The package defines a custom Unity's InjectionMember - InjectionConstructorLookup. It resolves a provided type between several inheritors of some basis type, depending on their constructor parameter signatures.

Each inheritor should have a constructor with a certain signature - each parameter should be inherited from a type passed to InjectionCOnstructorLookup's constructor.

Here is an example. Suppose we have following types:

```
interface IConfig { ... }
interface IReport { ... }

class ConfigA : IConfig { ... }
class ConfigB : IConfig { ... }

class ReportA : IReport
{
   public ReportA (ConfigA config)
   { ... }
}

class ReportB : IReport
{
   public ReportB (ConfigB config)
   { ... }
}
```

The problem is - we have a IConfig as an input and we want to decide which report to create.
Suppose then, we have registered both reports:

```
var unityContainer = new UnityContainer();

unityContainer.RegisterType<IReport, ReportA>("ReportA");
unityContainer.RegisterType<IReport, ReportB>("ReportB");
```

Do make things work we just add another registration of `IReport` with the `InjectionMember`:

   unityContainer.RegisterType<IReport>(new InjectionConstructorLookup<IConfig>());

Here `IConfig` type parameters says that we'll look for `IReport`-inherited objects with a single constructor argument, inherited from `IConfig`.

Then there are two ways of resolving `IReport`.

   var reportA = unityContainer.Resolve<IReport>(new DependencyOverride<IConfig>(new ConfigA())); // Should be ReportA
   var reportB = unityContainer.Resolve<IReport>(new DependencyOverride<IConfig>(new ConfigB())); // Should be ReportB

The second one is a delegate factory:

```
var factory = unityContainer.Resolve<Func<IConfig, IReport>>();

var reportA1 = factory(new ConfigA()); // Should be ReportA
var reportB1 = factory(new ConfigB()); // Should be ReportB
```

Call to the factory is faster due to result caching.

