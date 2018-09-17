using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace ServiceVirtualization.Database.SqlClient
{
    public static class ExtensionMethods
    {
        public static void AddWithValue(this DbParameterCollection parameterCollection, string parameterName, object value)
        {
            if (parameterCollection is ServiceVirtDbParameterCollection svParameterCollection)
            {
                svParameterCollection.AddWithValue(parameterName, value);
            }

            if (parameterCollection is SqlParameterCollection sqlParameterCollection)
            {
                sqlParameterCollection.AddWithValue(parameterName, value);
            }
        }

        public static void Add(this DbParameterCollection command, string parameterName, DbType type)
        {
            if (command is ServiceVirtDbParameterCollection svParameterCollection)
            {
                svParameterCollection.Add(parameterName);
            }

            if (command is SqlParameterCollection sqlParameterCollection)
            {
                sqlParameterCollection.Add(parameterName);
            }
        }
    }
}
