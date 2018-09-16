using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceVirtualization.Database.SqlClient
{
    public class ServiceVirtRecordingDbCommand : DbCommand
    {
        private SqlCommand _wrappedSqlCommand;
        private SqlConnection _wrappedSqlConnection;

        public ServiceVirtRecordingDbCommand()
        {
            DbParameterCollection = new ServiceVirtDbParameterCollection();
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; }

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override void Cancel() { }

        public override int ExecuteNonQuery()
        {
            HandleConnectionNull();

            CreateSqlCommand();

            CreateSqlParamList();

            int rowsAffected = 0;

            using (_wrappedSqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (_wrappedSqlCommand)
                {
                    _wrappedSqlCommand.Connection = _wrappedSqlConnection;
                    _wrappedSqlConnection.Open();

                    var sw = Stopwatch.StartNew();

                    rowsAffected = _wrappedSqlCommand.ExecuteNonQuery();

                    sw.Stop();

                    var virtModel = new VirtSqlRpcModel(CommandText)
                    {
                        DelayMs = sw.ElapsedMilliseconds,
                        RowsAffected = rowsAffected
                    };

                    MapOutParametersAndReturnValue(virtModel);

                    ServiceVirtMountebankPublisher.AddRecordedCommand(virtModel);
                }
            }

            return rowsAffected;
        }

        private void MapOutParametersAndReturnValue(VirtSqlRpcModel virtModel)
        {
            foreach (SqlParameter sqlParameter in _wrappedSqlCommand.Parameters)
            {
                if (sqlParameter.Direction == ParameterDirection.ReturnValue)
                {
                    virtModel.ReturnCode = (int)(sqlParameter.Value??0);
                    continue;
                }

                virtModel.Parameters.Add(sqlParameter.ParameterName, sqlParameter.Value);
            }
        }

        private void HandleConnectionNull()
        {
            if (Connection == null) throw new ArgumentNullException(nameof(Connection));
        }

        private void CreateSqlCommand()
        {
            _wrappedSqlCommand = new SqlCommand(CommandText);
            _wrappedSqlCommand.CommandType = CommandType;
        }

        private void CreateSqlParamList()
        {
            foreach (DbParameter parameter in DbParameterCollection)
            {
                var sqlParam = _wrappedSqlCommand.CreateParameter();

                sqlParam.ParameterName = parameter.ParameterName;
                sqlParam.DbType = parameter.DbType;
                sqlParam.Size = parameter.Size;
                sqlParam.Direction = parameter.Direction;
                sqlParam.Value = parameter.Value;

                _wrappedSqlCommand.Parameters.Add(sqlParam);
            }

            var returnValue = _wrappedSqlCommand.CreateParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;

            _wrappedSqlCommand.Parameters.Add(returnValue);
        }

        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) {
            return Task.FromResult(ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            return null;
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(default(object));
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) {
            return Task.FromResult(ExecuteDbDataReader(behavior));
        }

        public override void Prepare() { }

        protected override DbParameter CreateDbParameter()
        {
            return new ServiceVirtDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            HandleConnectionNull();

            CreateSqlCommand();

            CreateSqlParamList();

            var dataSet = new DataSet();

            using (_wrappedSqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (_wrappedSqlCommand)
                {
                    _wrappedSqlCommand.Connection = _wrappedSqlConnection;
                    _wrappedSqlConnection.Open();

                    var virtModel = new VirtSqlRpcModel(CommandText);

                    var sw = Stopwatch.StartNew();

                    using (var reader = _wrappedSqlCommand.ExecuteReader()) {
                         
                        do {

                            var schema = CreateSchemaTable(reader);

                            var data = CreateAndLoadDataTable(reader);

                            AddResultToVirtModel(virtModel, data, schema);

                            dataSet.Tables.Add(data);

                        } while (!reader.IsClosed);
                    }

                    MapOutParametersAndReturnValue(virtModel);

                    sw.Stop();

                    virtModel.DelayMs = sw.ElapsedMilliseconds;

                    ServiceVirtMountebankPublisher.AddRecordedCommand(virtModel); 
                }
            }

            return dataSet.CreateDataReader();
        }

        private static void AddResultToVirtModel(VirtSqlRpcModel virtModel, DataTable data, DataTable schema) {
            virtModel.Results.Add(new Result {Data = data, SchemaTable = schema});
        }
         
        private static DataTable CreateAndLoadDataTable(SqlDataReader reader) {
            var data = new DataTable();
            data.Load(reader);
            return data;
        }

        private static DataTable CreateSchemaTable(SqlDataReader reader) {
            var schema = reader.GetSchemaTable();
            return schema;
        }
    }
}