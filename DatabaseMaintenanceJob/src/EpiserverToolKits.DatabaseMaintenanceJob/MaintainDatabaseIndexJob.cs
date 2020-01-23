using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using System.Globalization;
using System.Linq;
using System.Text;


namespace EpiserverToolkits.DatabaseMaintenanceJob
{
    [ScheduledPlugIn(DisplayName = "MaintainDatabaseIndexJob",
        Description = "This job can be used to rebuild or reorganize all indices in both CMS and Commerce databases.",
        LanguagePath = "/scheduledjobs/maintaindatabaseindexjob", SortIndex = int.MaxValue)]
    public class MaintainDatabaseIndexJob : ScheduledJobBase
    {
        private readonly ILogger _log = LogManager.GetLogger(typeof(MaintainDatabaseIndexJob));

        private bool _stopSignaled;


        private readonly IDbIndexJobSettingLocator _dbIndexJobSettingLocator;
        private readonly IDbIndexOptimizer _dbIndexOptimizer;
        private readonly LocalizationService _localizationService;

        public MaintainDatabaseIndexJob(LocalizationService localizationService) : this(localizationService, new DbIndexJobSettingLocator(), new DbIndexOptimizer(new DbIndexJobSettingLocator()))
        {
            IsStoppable = true;
        }

        internal MaintainDatabaseIndexJob(LocalizationService localizationService, IDbIndexJobSettingLocator dbIndexJobSettingLocator, IDbIndexOptimizer dbIndexOptimizer)
        {
            _dbIndexJobSettingLocator = dbIndexJobSettingLocator;
            _dbIndexOptimizer = dbIndexOptimizer;
            _localizationService = localizationService;
        }


        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }


        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            StringBuilder sb = new StringBuilder();
            TotalOptimizedDbs = 0;
            //Call OnStatusChanged to periodically notify progress of job for manually started jobs
            OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            //Add implementation     
            var jobValidators = new JobSettingValidator(_dbIndexJobSettingLocator);
            if (!jobValidators.Validate())
            {
                return "ConnectionString settings are missed";
            }

            //For long running jobs periodically check if stop is signaled and if so stop execution
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }


            foreach (var conn in jobValidators.GetConnectionStrings())
            {
                var dbName = _dbIndexOptimizer.Optimize(conn.ConnectionString);

                string text = string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("/scheduledjobs/maintaindatabaseindexjob/successmessage"), dbName);
                sb.AppendFormat("{0}<br/>", text);
                TotalOptimizedDbs++;
            }

            if(TotalOptimizedDbs == 0 && _dbIndexJobSettingLocator.ConnectionStringSettings.Any())
            {
                return "All databases are currently excluded from index optmization";
            }
            return sb.ToString();
        }

        internal int TotalOptimizedDbs { get; set; }
    }
}
