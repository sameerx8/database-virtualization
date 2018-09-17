
using System.Data;
using System.Data.Common;

namespace ServiceVirtualization.Database.SqlClient
{
    public class ServiceVirtTransaction : DbTransaction
    {
        public override void Commit() { }

        public override void Rollback() { }

        protected override DbConnection DbConnection { get; }

        public override IsolationLevel IsolationLevel { get; }
    }
}