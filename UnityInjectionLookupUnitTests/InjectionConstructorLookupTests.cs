using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using UnityInjectionLookup;

namespace UnityInjectionLookupUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private interface IConfig { }

        private class ConfigA : IConfig { }
        private class ConfigB : IConfig { }

        private interface IReport { }

        private class ReportA : IReport
        {
            public ReportA(ConfigA config) { }
        }
        private class ReportB : IReport
        {
            public ReportB(ConfigB config) { }
        }

        [TestMethod]
        public void TestFactory()
        {
            var container = new UnityContainer();

            container.RegisterType<IConfig, ConfigA>("A");
            container.RegisterType<IConfig, ConfigB>("B");
            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            var factory = container.Resolve<Func<IConfig, IReport>>("Select");

            Assert.IsInstanceOfType(factory(new ConfigA()), typeof(ReportA));
            Assert.IsInstanceOfType(factory(new ConfigB()), typeof(ReportB));
        }

        [TestMethod]
        public void TestDefaultConfigResolve()
        {
            var container = new UnityContainer();

            container.RegisterType<IConfig, ConfigB>();
            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            Assert.IsInstanceOfType(container.Resolve<IReport>("Select"), typeof(ReportB));
        }

        [TestMethod]
        public void TestOverriddenResolve()
        {
            var container = new UnityContainer();

            container.RegisterType<IReport, ReportA>("A");
            container.RegisterType<IReport, ReportB>("B");
            container.RegisterType<IReport>("Select", new InjectionConstructorLookup<IConfig>());

            Assert.IsInstanceOfType(container.Resolve<IReport>("Select", new DependencyOverride<IConfig>(new ConfigB())), typeof(ReportB));
        }
    }
}
