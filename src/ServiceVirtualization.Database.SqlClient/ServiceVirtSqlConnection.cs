using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceVirtualization.Database.SqlClient;

namespace ServiceVirtualization.Database.SqlClient
{
    public enum DatabaseVirtualizationMode
    {
        None,
        Record,
        Playback
    }

    public interface ISqlConnectionFactory
    {
        DbConnection CreateSqlConnection(string connectionString);
    }

    public class SqlServerConnectionFactory : ISqlConnectionFactory
    {
        public DbConnection CreateSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }

    public class ServiceVirtsqlConnectionFactory : ISqlConnectionFactory
    {
        public DbConnection CreateSqlConnection(string connectionString)
        {
            return new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Record);
        }
    }

    public class ServiceVirtSqlConnection : DbConnection
    {
        private readonly DatabaseVirtualizationMode _mode;

        public ServiceVirtSqlConnection(string connectionString, DatabaseVirtualizationMode mode)
        {
            _mode = mode;
            ConnectionString = connectionString;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ServiceVirtTransaction();
        }

        public override void ChangeDatabase(string databaseName) { }

        public override void Close() { }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override void Open() { }

        public override string ConnectionString { get; set; }

        public override string Database { get; }

        public override ConnectionState State { get; }

        public override string DataSource { get; }

        public override string ServerVersion { get; }

        protected override DbCommand CreateDbCommand()
        {
            switch (_mode)
            {
                case DatabaseVirtualizationMode.Playback:
                    return new ServiceVirtPlaybackDbCommand();

                case DatabaseVirtualizationMode.Record:
                    return new ServiceVirtRecordingDbCommand();

                default:
                    throw new NotSupportedException();
            }
        }
    }

}

