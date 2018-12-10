# Overview [![NuGet](https://img.shields.io/nuget/v/UnityInjectionLookup.svg)](https://www.nuget.org/packages/UnityInjectionLookup) [![Build Status](https://travis-ci.com/stop-cran/UnityInjectionLookup.svg?branch=master)](https://travis-ci.com/stop-cran/UnityInjectionLookup)

The package defines a custom `InjectionMember` of [Unity Container](https://github.com/unitycontainer/unity) - `InjectionConstructorLookup`. It resolves a provided type between several inheritors of some basis type, depending on their constructor parameter signatures.

Each inheritor should have a constructor with a certain signature - each parameter should be inherited from a type passed to constructor of `InjectionConstructorLookup`.

# Installation

NuGet package is available [here](https://www.nuget.org/packages/UnityInjectionLookup/).

```PowerShell
PM> Install-Package UnityInjectionLookup
```

# Example

Suppose we have following types:

```C#
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

The problem is - we have a `IConfig` as an input and we want to decide which report to create.
Suppose then, we have registered both reports:

```C#
var unityContainer = new UnityContainer();

unityContainer.RegisterType<IReport, ReportA>("ReportA");
unityContainer.RegisterType<IReport, ReportB>("ReportB");
```

Do make things work we just add another registration of `IReport` with the `InjectionMember`:

```C#
unityContainer.RegisterType<IReport>(new InjectionConstructorLookup<IConfig>());
```

Here `IConfig` type parameters says that we'll look for `IReport`-inherited objects with a single constructor argument, inherited from `IConfig`.

Then there are two ways of resolving `IReport`.

```C#
var reportA = unityContainer.Resolve<IReport>(new DependencyOverride<IConfig>(new ConfigA())); // Should be ReportA
var reportB = unityContainer.Resolve<IReport>(new DependencyOverride<IConfig>(new ConfigB())); // Should be ReportB
```

The second one is a delegate factory:

```C#
var factory = unityContainer.Resolve<Func<IConfig, IReport>>();

var reportA1 = factory(new ConfigA()); // Should be ReportA
var reportB1 = factory(new ConfigB()); // Should be ReportB
```

Call to the factory is faster due to result caching.

