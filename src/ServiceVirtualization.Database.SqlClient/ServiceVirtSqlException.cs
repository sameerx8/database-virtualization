using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace ServiceVirtualization.Database.SqlClient
{
    public class ServiceVirtSqlException
    {
        public static SqlException CreateSqlException(string errorMessage, int errorNumber, int state = 0, int severity = 10,
            int lineNumber = 0, string server = "ServiceVirt", string procedureName = "ServiceVirtUnknownProcName")

        {

            SqlErrorCollection collection = GetErrorCollection();
            SqlError error = GetError(errorNumber, errorMessage, state, severity, lineNumber, server, procedureName);
            MethodInfo addMethod = collection.GetType().GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);

            addMethod.Invoke(collection, new object[] { error });

            Type[] types = new Type[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) };

            object[] parameters = new object[] { errorMessage, collection, null, Guid.NewGuid() };

            ConstructorInfo constructor = typeof(SqlException).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
            SqlException exception = (SqlException)constructor.Invoke(parameters);

            return exception;
        }

        private static SqlError GetError(int errorCode, string message, int state=0, int severity=10, 
            int lineNumber=0, string server="ServiceVirt", string procedureName="ServiceVirtUnknownProcName")
        {
            //  internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber) 
            object[] parameters = new object[] { errorCode, (byte)state, (byte)severity, server, message, procedureName, lineNumber, null };

            Type[] types = new Type[] {
                typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string),
                typeof(string), typeof(int), typeof(Exception) };


            ConstructorInfo constructor = typeof(SqlError).GetConstructor(BindingFlags.NonPublic| BindingFlags.Instance, null, types, null);

            SqlError error = (SqlError)constructor.Invoke(parameters);

            return error;
        }



        private static SqlErrorCollection GetErrorCollection()
        {

            ConstructorInfo constructor = typeof(SqlErrorCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

            SqlErrorCollection collection = (SqlErrorCollection)constructor.Invoke(new object[] { });

            return collection;
        }
    }
}
