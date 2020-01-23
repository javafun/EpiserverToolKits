using EpiserverToolkits.DatabaseMaintenanceJob;
using Moq;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Xunit;

namespace EpiserverToolKits.Tests
{
    [Trait("Index rebuilt ran successfully", "")]
    public class IndexRebuiltSuccessfullyTests : IndexRebuildJobTestBase
    {
        private MaintainDatabaseIndexJob job;

        public IndexRebuiltSuccessfullyTests()
        {
            job = new MaintainDatabaseIndexJob(providerBasedLocalizationService, dbIndexJobSettingLocatorMock.Object, dbOptimizerMock.Object);
        }

        [Fact(DisplayName = "A success result is sent back")]
        public void Should_respond_with_success_status()
        {
            var result = job.Execute();
            Assert.Equal(1, job.TotalOptimizedDbs);
        }
    }
    [Trait("Index rebuilt ran with invalid settings", "")]
    public class IndexRebuildWithInvalidSettingsTests : IndexRebuildJobTestBase
    {
        private MaintainDatabaseIndexJob job;

        public IndexRebuildWithInvalidSettingsTests()
        {
            dbIndexJobSettingLocatorMock.SetupGet(x => x.ConnectionStringSettings).Returns(Enumerable.Empty<ConnectionStringSettings>());
            job = new MaintainDatabaseIndexJob(providerBasedLocalizationService, dbIndexJobSettingLocatorMock.Object, dbOptimizerMock.Object);
        }

        [Fact(DisplayName = "A fail result is sent back")]
        public void Should_respond_with_fail_message()
        {
            var result = job.Execute();

            Assert.Equal("ConnectionString settings are missed", result);
            Assert.Equal(0, job.TotalOptimizedDbs);
        }
    }

    [Trait("Index rebuilt with all connectionstrings excluded", "")]
    public class IndexRebuildWithAllConnectionstringsExcludedTests : IndexRebuildJobTestBase
    {
        private MaintainDatabaseIndexJob job;

        public IndexRebuildWithAllConnectionstringsExcludedTests()
        {
            dbIndexJobSettingLocatorMock.SetupGet(x => x.ExcludedDatabases).Returns(new List<string> { "LocalSqlServer" });
            dbIndexJobSettingLocatorMock.SetupGet(x => x.ConnectionStringSettings)
                .Returns(new List<ConnectionStringSettings> { new ConnectionStringSettings
                {
                    ConnectionString = It.IsAny<string>(),
                    Name = "LocalSqlServer"
                } });

            job = new MaintainDatabaseIndexJob(providerBasedLocalizationService, dbIndexJobSettingLocatorMock.Object, dbOptimizerMock.Object);
        }

        [Fact(DisplayName = "No database indexes should be rebuilt or reorganized")]
        public void Should_not_execute_anything()
        {
            var result = job.Execute();
            Assert.Equal(0, job.TotalOptimizedDbs);
            Assert.Equal("All databases are currently excluded from index optmization", result);
        }
    }
}
