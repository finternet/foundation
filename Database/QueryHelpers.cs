using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.Foundation.Database
{
    public static class QueryHelpers
    {
        public static string GetConnectionString(string connectionName)
        {
            try
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Shortcut method to get Connection Object from connection string
        /// </summary>
        /// <param name="connectionString">Connection String to be used</param>
        /// <returns>The connection object</returns>
        public static System.Data.SqlClient.SqlConnection GetConnection(string connectionString)
        {
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        /// <summary>
        /// Runs a stored procedure and returns the value of the first row and column in the result
        /// </summary>
        /// <param name="connectionString">Connection String to be used</param>
        /// <param name="storedProcedureName">Name of stored procedure to run</param>
        /// <param name="returnValueHandler">If you have Out or InOut parameters, here you can pass callback to handle and read it</param>
        /// <param name="parameters">Parameters to be passed to stored procedure</param>
        /// <returns>Returns the first value on the first row of the first result set</returns>
        public static object ExecuteSPROCScalar(string connectionString, string storedProcedureName, Action<System.Data.SqlClient.SqlCommand> returnValueHandler, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            using (var cn = GetConnection(connectionString))
            {
                try
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = storedProcedureName;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        cn.Open();
                        var result = cmd.ExecuteScalar();
                        if (returnValueHandler != null)
                        {
                            returnValueHandler(cmd);
                        }
                        return result;
                    }
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        /// <summary>
        /// Runs an stored procedure and allows you to handle the result using the DataReader 
        /// </summary>
        /// <param name="connectionString">Connection string to be used for accessing DB</param>
        /// <param name="storedProcedureName">Name of stored procedure to run</param>
        /// <param name="resultHandler">The callback method which should receive DataReader object</param>
        /// <param name="returnValueHandler">The callback method to handle return values on parameters (Out or InOut parameters)</param>
        /// <param name="parameters">Parameters to be passed to stored procedure</param>
        public static void ExecuteSPROCReader(string connectionString, string storedProcedureName, Action<System.Data.SqlClient.SqlDataReader> resultHandler, Action<System.Data.SqlClient.SqlCommand> returnValueHandler, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            using (var cn = GetConnection(connectionString))
            {
                try
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = storedProcedureName;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        cn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            resultHandler(dr);
                        }
                        if (returnValueHandler != null)
                        {
                            returnValueHandler(cmd);
                        }
                    }
                }
                finally
                {
                    cn.Close();
                }
            }
        }
    }
}
