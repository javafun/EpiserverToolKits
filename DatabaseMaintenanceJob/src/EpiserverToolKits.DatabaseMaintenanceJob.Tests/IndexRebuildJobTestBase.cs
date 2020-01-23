using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.Internal;
using EpiserverToolkits.DatabaseMaintenanceJob;
using Moq;
using System.Collections.Generic;
using System.Configuration;

namespace EpiserverToolKits.Tests
{
    public class IndexRebuildJobTestBase
    {
        protected Mock<IDbIndexJobSettingLocator> dbIndexJobSettingLocatorMock;
        protected ProviderBasedLocalizationService providerBasedLocalizationService;
        protected Mock<IDbIndexOptimizer> dbOptimizerMock;

        public IndexRebuildJobTestBase()
        {
            providerBasedLocalizationService = new ProviderBasedLocalizationService();
            providerBasedLocalizationService.AddProvider(new MemoryLocalizationProvider());

            dbIndexJobSettingLocatorMock = new Mock<IDbIndexJobSettingLocator>();
            dbIndexJobSettingLocatorMock.SetupGet(x => x.ExcludedDatabases).Returns(new List<string> { "LocalSqlServer" });
            dbIndexJobSettingLocatorMock.SetupGet(x => x.ConnectionStringSettings)
                .Returns(new List<ConnectionStringSettings> { new ConnectionStringSettings
                {
                    ConnectionString = It.IsAny<string>(),
                    Name = "LocalSqlServer2"
                } });
            dbOptimizerMock = new Mock<IDbIndexOptimizer>();
            dbOptimizerMock.Setup(x => x.Optimize(It.IsAny<string>())).Returns("something");
        }
    }
}
