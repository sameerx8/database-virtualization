using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MbDotNet;
using Newtonsoft.Json;

namespace ServiceVirtualization.Database.SqlClient
{
    public class ServiceVirtPlaybackDbCommand : DbCommand
    {
        private SqlCommand _wrappedSqlCommand;
        private SqlConnection _wrappedSqlConnection;
        private readonly HttpClient virtualServiceHttpClient;

        public ServiceVirtPlaybackDbCommand()
        {
            DbParameterCollection = new ServiceVirtDbParameterCollection();
            virtualServiceHttpClient = new HttpClient();
            _wrappedSqlCommand = new SqlCommand();
            virtualServiceHttpClient.BaseAddress = new Uri("http://localhost:1234");
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


        public override int ExecuteNonQuery() {
            return ExecuteNonQueryAsync(CancellationToken.None).Result;
        }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) {
            var virtModel = await ProcessVirtService(cancellationToken);

            return virtModel.RowsAffected;
        }

        private async Task<VirtSqlRpcModel> ProcessVirtService(CancellationToken cancellationToken) {
            var response = await virtualServiceHttpClient.GetAsync(CommandText, cancellationToken);

            var virtModel = JsonConvert.DeserializeObject<VirtSqlRpcModel>(await response.Content.ReadAsStringAsync());

            if (virtModel == null) throw new InvalidOperationException("Virtual service returned invalid response");

            if (virtModel.DelayMs > 0) await Task.Delay((int) virtModel.DelayMs, cancellationToken);

            if (virtModel.Error != null) {
                throw ServiceVirtSqlException.CreateSqlException(virtModel.Error.Message, virtModel.Error.Code,
                    virtModel.Error.State, virtModel.Error.Severity, virtModel.Error.LineNumber, virtModel.Error.Server,
                    virtModel.Error.ProcedureName);
            }

            foreach (DbParameter parameter in DbParameterCollection) {
                if (virtModel.Parameters.ContainsKey(parameter.ParameterName)) {
                    parameter.Value = virtModel.Parameters[parameter.ParameterName];
                }

                if (parameter.Direction == ParameterDirection.ReturnValue) {
                    parameter.Value = virtModel.ReturnCode;
                }
            }

            return virtModel;
        }

        public override object ExecuteScalar()
        {
            return null;
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(default(object));
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) {
            var virtModel = await ProcessVirtService(cancellationToken);

            var ds = new DataSet();

            foreach (var virtModelResult in virtModel.Results) {
                var schemaTable = new DataTable();

                foreach (DataRow row in virtModelResult.SchemaTable.Rows) {
                    var columnName = row["ColumnName"].ToString();
                    var dataType = Type.GetType(row["DataType"].ToString());
                      
                    schemaTable.Columns.Add(columnName, dataType);
                }

                foreach (DataRow row in virtModelResult.Data.Rows) {
                    schemaTable.Rows.Add(row.ItemArray);
                }

                ds.Tables.Add(schemaTable);
            } 

            return ds.CreateDataReader();
        }

        public override void Prepare() { }

        protected override DbParameter CreateDbParameter()
        {
            return new ServiceVirtDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            return ExecuteDbDataReaderAsync(behavior, CancellationToken.None).Result;
        }
    }
}