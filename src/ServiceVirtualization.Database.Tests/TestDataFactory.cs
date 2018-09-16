using System;
using System.Collections.Generic;
using System.Text;
using ServiceVirtualization.Database.SqlClient;

namespace ServiceVirtualization.Database.Tests {
    public class TestDataFactory {
        public static VirtSqlRpcModel CreateVirtTpcVirtModel()
        {
            var virtModel = new VirtSqlRpcModel
            {
                StoredProcName = "bob.pr_bobby",
                Parameters = new Dictionary<string, object> {
                    {"@UserId", 43543},
                    {"@UserTransNumber", 122}
                },
                ReturnCode = -5,
                Error = new Error
                {
                    Code = 1,
                    Severity = 20,
                    State = 1,
                    LineNumber = 324,
                    Message = "Error has occurred - Service Virt",
                    ProcedureName = "bob.pr_bobby"
                },
                Resultsets = new List<List<Dictionary<string, object>>> {
                    new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {"firstName", "sameer"},
                            { "Balance", 234.43m}
                        },
                        new Dictionary<string, object> {
                            {"firstName", "meher"},
                            { "Balance", 5345.4m}
                        }
                    },
                    new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {"id", 342},
                            {"IsEnabled", true},
                            {"ProductId", 5001},
                            {"rowId", Guid.NewGuid()},
                            {"Balance", 5345.4m},
                            {"PasswordHash", Encoding.UTF8.GetBytes("balla")}
                        },
                        new Dictionary<string, object>
                        {
                            {"id", 453534},
                            {"IsEnabled", false},
                            {"ProductId", 5004},
                            {"rowId", Guid.NewGuid()},
                            {"Balance", 5345.4m},
                            {"PasswordHash", Encoding.UTF8.GetBytes("sdfasdf")}
                        }
                    }
                },
                DelayMs = 43
            };
            return virtModel;
        }
    }
}