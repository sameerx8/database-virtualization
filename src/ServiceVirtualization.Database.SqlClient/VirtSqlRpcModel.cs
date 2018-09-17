using System.Collections.Generic;
using System.Data;

namespace ServiceVirtualization.Database.SqlClient
{
    public class VirtSqlRpcModel
    {
        public VirtSqlRpcModel()
        {
            Parameters = new Dictionary<string, object>();
            Resultsets = new List<List<Dictionary<string, object>>>();
            Results = new List<Result>();
        }

        public VirtSqlRpcModel(string storedProcName) : this()
        {
            StoredProcName = storedProcName;

        }

        public string StoredProcName { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public IEnumerable<IEnumerable<Dictionary<string, object>>> Resultsets { get; set; }

        public List<Result> Results { get; set; }

        public int RowsAffected { get; set; }

        public int ReturnCode { get; set; }

        public Error Error { get; set; }

        public long DelayMs { get; set; }
    }

    public class Result
    {
        public DataTable SchemaTable { get; set; }
        public DataTable Data { get; set; }
    }
}