using EPiServer.Logging;
using System.Data;
using System.Data.SqlClient;

namespace EpiserverToolkits.DatabaseMaintenanceJob
{

    public interface IDbIndexOptimizer
    {
        string Optimize(string connStr);
    }

    public class DbIndexOptimizer : IDbIndexOptimizer
    {
        private readonly ILogger _log = LogManager.GetLogger(typeof(DbIndexOptimizer));

        private readonly IDbIndexJobSettingLocator _dbIndexJobSettingLocator;

        public DbIndexOptimizer(IDbIndexJobSettingLocator dbIndexJobSettingLocator)
        {
            _dbIndexJobSettingLocator = dbIndexJobSettingLocator;
        }
        public string Optimize(string connStr)
        {

            using (SqlConnection sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = CreateCommand(sqlConnection, sqlConnection.Database,
                            _dbIndexJobSettingLocator.LowFragmentationThreshold,
                            _dbIndexJobSettingLocator.HighFragmentationThreshold))
                {
                    sqlCommand.ExecuteNonQuery();
                }

                return sqlConnection.Database;
            }
        }


        private SqlCommand CreateCommand(SqlConnection connection, string databaseName, int lowThreshold, int highThreshold)
        {
            SqlCommand sqlCommand = new SqlCommand($"\r\n                DECLARE @sql NVARCHAR(1000)\r\n\r\n                DECLARE @indexName NVARCHAR(255)\r\n\r\n                DECLARE @tableName NVARCHAR(255)\r\n\r\n                DECLARE @fragmentation FLOAT\r\n\r\n                DECLARE @msg NVARCHAR(MAX)\r\n\r\n                DECLARE @dbId INT\r\n\r\n                DECLARE @indexCount INT\r\n\r\n\r\n                SET NOCOUNT ON\r\n\r\n\r\n                SET @dbId = db_id('{databaseName}')\r\n\r\n                IF @dbId IS NULL\r\n\r\n                BEGIN\r\n\r\n                    SET @msg = N'The database ' + '{databaseName}' + ' does not exist!'\r\n\r\n                    RAISERROR(@msg, 0, 1) WITH NOWAIT\r\n\r\n                    RETURN;\r\n            END\r\n\r\n            SET @indexCount = 0\r\n\r\n\r\n                DECLARE c CURSOR FOR\r\n\r\n                SELECT 'ALTER INDEX [' + i.name + '] ON ' + '[' + object_schema_name(d.object_id, @dbId) + '].[' + object_name(d.object_id) + ']' + \r\n                    CASE WHEN avg_fragmentation_in_percent > {highThreshold} OR i.allow_page_locks = 0\r\n                         THEN ' REBUILD' \r\n                         ELSE ' REORGANIZE' \r\n                         END \r\n                    AS [sql],\r\n                        convert(decimal(5, 2), avg_fragmentation_in_percent) fragmentation, object_name(d.object_id), i.name\r\n                FROM sys.dm_db_index_physical_stats(@dbId, NULL, -1, NULL, 'SAMPLED') d\r\n\r\n                INNER JOIN sys.indexes i ON i.object_id = d.object_id AND i.index_id = d.index_id\r\n\r\n                WHERE d.avg_fragmentation_in_percent > {lowThreshold}\r\n\r\n                ORDER BY avg_fragmentation_in_percent DESC\r\n\r\n                OPEN c\r\n\r\n                FETCH NEXT FROM c INTO @sql, @fragmentation, @tableName, @indexName\r\n\r\n                WHILE @@FETCH_STATUS = 0\r\n\r\n                BEGIN\r\n                    EXEC sp_executesql @sql\r\n                    SET @indexCount = @indexCount + 1\r\n\r\n                    FETCH NEXT FROM c INTO @sql, @fragmentation, @tableName, @indexName\r\n                END\r\n\r\n                CLOSE c\r\n                DEALLOCATE c", connection);
            sqlCommand.CommandType = CommandType.Text;
            if (_dbIndexJobSettingLocator.DataBaseIndicesJobCommandTimeOut > 0)
            {
                sqlCommand.CommandTimeout = _dbIndexJobSettingLocator.DataBaseIndicesJobCommandTimeOut;
            }

            _log.Information(sqlCommand.CommandText);
            return sqlCommand;
        }
    }
}
