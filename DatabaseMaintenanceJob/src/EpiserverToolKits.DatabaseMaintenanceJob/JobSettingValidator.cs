using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EpiserverToolkits.DatabaseMaintenanceJob
{
    public class JobSettingValidator
    {
        private readonly IDbIndexJobSettingLocator _dbIndexJobSettingLocator;

        public JobSettingValidator(IDbIndexJobSettingLocator dbIndexJobSettingLocator)
        {
            _dbIndexJobSettingLocator = dbIndexJobSettingLocator;
        }

        public bool Validate()
        {
            if (_dbIndexJobSettingLocator.ConnectionStringSettings == null || !_dbIndexJobSettingLocator.ConnectionStringSettings.Any())
                return false;
            return true;
        }

        public IEnumerable<ConnectionStringSettings> GetConnectionStrings()
        {
            return _dbIndexJobSettingLocator.ConnectionStringSettings.Where(x => !_dbIndexJobSettingLocator.ExcludedDatabases.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase));
        }
    }
}
