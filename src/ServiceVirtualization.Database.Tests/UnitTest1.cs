using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using NSubstitute;
using ServiceVirtualization.Database.SqlClient;
using Xunit;

namespace ServiceVirtualization.Database.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var exception = ServiceVirtSqlException.CreateSqlException("balla", 32, 1, 20, 433);

        }

        [Fact]
        public void TesVirtRpcModel_ShouldSerializeandDeserialized() {
            var virtModel = TestDataFactory.CreateVirtTpcVirtModel();

            var data = JsonConvert.SerializeObject(virtModel);

            JsonConvert.DeserializeObject<VirtSqlRpcModel>(data);
        }


        [Fact]

        public void ExecuteNonQuery_SqlCommandWithSingleOutputParameter_ShouldMapOutputParameterValue() {
            var virtModel = TestDataFactory.CreateVirtTpcVirtModel();


            var command = new ServiceVirtRecordingDbCommand();

            command.CommandText = "application.CreateRoleIfNonexistent";
            command.CommandType = CommandType.StoredProcedure;

            var roleName = command.CreateParameter();
            roleName.ParameterName = "@RoleName";
            roleName.DbType = DbType.String;
            roleName.Size = 50;
            roleName.Direction = ParameterDirection.Input;

            var roleId = command.CreateParameter();
            roleName.ParameterName = "@RoleId";
            roleName.DbType = DbType.Int32;
            roleName.Size = 4;
            roleName.Direction = ParameterDirection.Output;

            command.Parameters.Add(roleName);
            command.Parameters.Add(roleId);

            using (var connection = new SqlConnection("server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$"))
            {
                command.Connection = connection;
                connection.Open();

                //command.ExecuteNonQuery();

                //Assert.InRange((int) command.Parameters["@roleId"].Value, 0, 100);
            }

            var param = new SqlParameter("test",SqlDbType.BigInt);

            var dat = (DbParameter) param;



            //command.ExecuteNonQuery();

            

        }

        [Fact]
        public void SqlToVirtAdapter_ExecuteNonQuery() {
            var connectionString = "server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$;MultipleActiveResultSets=True";

            var connectionFactory = Substitute.For<ISqlConnectionFactory>();

            connectionFactory.CreateSqlConnection(Arg.Any<string>()).Returns(new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Record));


            using (var connection = connectionFactory.CreateSqlConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand()) {
                    cmd.CommandText = "application.CreateRoleIfNonexistent";

                    cmd.Connection = connection;

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleName", "ballasyd3");
                    
                    var roleIdParam = cmd.CreateParameter();
                    roleIdParam.DbType = DbType.Int32;
                    roleIdParam.Size = 4;
                    roleIdParam.ParameterName= "@RoleId";
                    roleIdParam.Direction = ParameterDirection.Output; 

                    cmd.Parameters.Add(roleIdParam);

                    connection.Open();

                    cmd.ExecuteNonQuery(); 
                }
            }
        }

        [Fact]
        public void SqlToVirtAdapter_ExecuteResultset() {
            var connectionString = "server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$;MultipleActiveResultSets=True";

            var connectionFactory = Substitute.For<ISqlConnectionFactory>();

            connectionFactory.CreateSqlConnection(Arg.Any<string>()).Returns(new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Record));

            using (var connection = connectionFactory.CreateSqlConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand()) 
                {
                    cmd.CommandText = "Integration.GetOrderUpdates";
                     
                    cmd.Connection = connection;
                     
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LastCutoff", DateTime.Parse("2016-05-01"));
                    cmd.Parameters.AddWithValue("@NewCutoff", DateTime.Parse("2016-05-03"));

                    connection.Open();
                     
                    var result = cmd.ExecuteReader();
                }
            }
        }

        [Fact]
        public void SqlToVirtAdapter_ExecuteResultsetPlayback() {
            var connectionString = "server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$;MultipleActiveResultSets=True";

            var connectionFactory = Substitute.For<ISqlConnectionFactory>();

            connectionFactory.CreateSqlConnection(Arg.Any<string>()).Returns(new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Playback));

            using (var connection = connectionFactory.CreateSqlConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "Integration.GetOrderUpdates";

                    cmd.Connection = connection;

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LastCutoff", DateTime.Parse("2016-05-01"));
                    cmd.Parameters.AddWithValue("@NewCutoff", DateTime.Parse("2016-05-03"));

                    connection.Open();
                     
                    var result = cmd.ExecuteReader();
                }
            }
        }

        [Fact]
        public void SqlToVirtAdapter_ExecuteNonQueryPlayback()
        {
            var connectionString = "server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$;MultipleActiveResultSets=True";

            var connectionFactory = Substitute.For<ISqlConnectionFactory>();

            connectionFactory.CreateSqlConnection(Arg.Any<string>()).Returns(new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Playback));


            using (var connection = connectionFactory.CreateSqlConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "application.CreateRoleIfNonexistent";

                    cmd.Connection = connection;

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleName", "ballasd");

                    var roleIdParam = cmd.CreateParameter();
                    roleIdParam.DbType = DbType.Int32;
                    roleIdParam.Size = 4;
                    roleIdParam.ParameterName = "@RoleId";
                    roleIdParam.Direction = ParameterDirection.Output; 

                    cmd.Parameters.Add(roleIdParam);

                    connection.Open();

                    cmd.ExecuteNonQuery();

                    
                }
            }
        }

        [Fact]
        public void SqlToVirtAdapter_ExecuteNonQueryPlaybackError()
        {
            var connectionString = "server=sameerpc;database=wideworldimporters;user id=sa;password=password1234$;MultipleActiveResultSets=True";

            var connectionFactory = Substitute.For<ISqlConnectionFactory>();

            connectionFactory.CreateSqlConnection(Arg.Any<string>()).Returns(new ServiceVirtSqlConnection(connectionString, DatabaseVirtualizationMode.Playback));


            using (var connection = connectionFactory.CreateSqlConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "sqlerror";

                    cmd.Connection = connection;

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleName", "ballasd");

                    var roleIdParam = cmd.CreateParameter();
                    roleIdParam.DbType = DbType.Int32;
                    roleIdParam.Size = 4;
                    roleIdParam.ParameterName = "@RoleId";
                    roleIdParam.Direction = ParameterDirection.Output; 

                    cmd.Parameters.Add(roleIdParam);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }



        [Fact]
        public void DictionaryListToDataTable() {
            var resultSets = new List<List<Dictionary<string, object>>> {
                new List<Dictionary<string, object>> {
                    new Dictionary<string, object> {
                        {"firstName", "sameer"},
                        {"Balance", 234.43m}
                    },
                    new Dictionary<string, object> {
                        {"firstName", "meher"},
                        {"Balance", 5345.4m}
                    }
                },
                new List<Dictionary<string, object>> {
                    new Dictionary<string, object> {
                        {"id", 342},
                        {"IsEnabled", true},
                        {"ProductId", 5001},
                        {"rowId", Guid.NewGuid()},
                        {"Balance", 5345.4m},
                        {"PasswordHash", Encoding.UTF8.GetBytes("balla")}
                    },
                    new Dictionary<string, object> {
                        {"id", 453534},
                        {"IsEnabled", false},
                        {"ProductId", 5004},
                        {"rowId", Guid.NewGuid()},
                        {"Balance", 5345.4m},
                        {"PasswordHash", Encoding.UTF8.GetBytes("sdfasdf")}
                    }
                }
            };

            var ds = new DataSet();

            foreach (var resultSet in resultSets) {
                var dt = new DataTable();
                var columns = resultSet.First();

                //foreach (var o in resultSet) {
                //    dt.Columns.Add(new DataColumn(o.Key));
                //}

                //foreach (var o in resultSet) {
                //    dt.Rows.Add(resultSet.Select(c => c.Value));
                //}
            }

        }
    }
}
