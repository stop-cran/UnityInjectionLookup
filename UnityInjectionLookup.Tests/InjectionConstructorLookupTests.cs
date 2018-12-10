using NUnit.Framework;
using Shouldly;
using System;
using Unity;
using Unity.Resolution;
using UnityInjectionLookup;

namespace UnityInjectionLookup.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private interface IConfig { }

        private class ConfigA : IConfig { }

        private class ConfigB : IConfig { }

        private interface IReport { }

        private class ReportA : IReport
        {
            public ReportA(ConfigA config)
            {
            }
        }

        private class ReportB : IReport
        {
            public ReportB(ConfigB config)
            {
            }
        }

        [Test]
        public void TestFactory()
        {
            var container = new UnityContainer();

            container.RegisterType<IConfig, ConfigA>("A");
            container.RegisterType<IConfig, ConfigB>("B");
            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            var factory = container.Resolve<Func<IConfig, IReport>>("Select");

            factory(new ConfigA()).ShouldBeOfType<ReportA>();
            factory(new ConfigB()).ShouldBeOfType<ReportB>();
        }

        [Test]
        public void TestDefaultConfigResolve()
        {
            var container = new UnityContainer();

            container.RegisterType<IConfig, ConfigB>();
            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            container.Resolve<IReport>("Select").ShouldBeOfType<ReportB>();
        }

        [Test]
        public void TestOverriddenResolve()
        {
            var container = new UnityContainer();

            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            container.Resolve<IReport>("Select", new DependencyOverride<IConfig>(new ConfigB()))
                .ShouldBeOfType<ReportB>();
        }
    }
}
