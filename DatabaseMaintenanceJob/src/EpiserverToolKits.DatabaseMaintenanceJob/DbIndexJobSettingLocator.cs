using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EpiserverToolkits.DatabaseMaintenanceJob
{
    public interface IDbIndexJobSettingLocator
    {
        IEnumerable<ConnectionStringSettings> ConnectionStringSettings { get; }
        int LowFragmentationThreshold { get; }
        int HighFragmentationThreshold { get; }
        int DataBaseIndicesJobCommandTimeOut { get; }
        IEnumerable<string> ExcludedDatabases { get; }
    }

    public class DbIndexJobSettingLocator : IDbIndexJobSettingLocator
    {
        public IEnumerable<ConnectionStringSettings> ConnectionStringSettings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>();
            }
        }

        /// <summary>
        /// If the avg_fragmentation_in_percent of an index is larger than this value,
        /// and smaller or equal to <see cref="HighFragmentationThreshold" /> value, then the index will be re-orgarnized.
        /// The default value is 10.
        /// </summary>
        public int LowFragmentationThreshold
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["lowFragmentationThreshold"], out int result))
                {
                    return 10;
                }
                return result;
            }
        }

        /// <summary>
        /// If the avg_fragmentation_in_percent of an index is larger than this value, 
        /// the index will be rebuilt. The default value is 30.
        /// </summary>
        public int HighFragmentationThreshold
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["highFragmentationThreshold"], out int result))
                {
                    return 30;
                }
                return result;
            }
        }


        public int DataBaseIndicesJobCommandTimeOut
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["dataBaseIndicesJobCommandTimeOut"], out int result))
                {
                    return -1;
                }
                return result;
            }
        }

        public IEnumerable<string> ExcludedDatabases
        {
            get
            {
                List<string> excludeDbList = new List<string>();
                excludeDbList.Add("LocalSqlServer");

                var excludedDbStr = ConfigurationManager.AppSettings["excludedDatabases"];
                if (!string.IsNullOrWhiteSpace(excludedDbStr))
                {
                    excludeDbList.AddRange(excludedDbStr.Split(',', ';'));
                }

                return excludeDbList;
            }
        }
    }
}
