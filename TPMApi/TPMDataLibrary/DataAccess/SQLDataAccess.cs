using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TPMDataLibrary.DataAccess
{
    class SQLDataAccess
    {
        /// <summary>
        /// Save data to database using DAPPER
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int SaveData<T>(string sql, T data, string sqlConn)
        {
            using (IDbConnection cnn = new SqlConnection(sqlConn))
            {
                return cnn.Execute(sql, data);
            }
        }

        /// <summary>
        /// Load data from DB using dapper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> LoadData<T>(string sql, string sqlConn)
        {
            using (IDbConnection cnn = new SqlConnection(sqlConn))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }

        /// <summary>
        /// Compare specific data.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CompareData(string sql, string data, string sqlConn)
        {
            using (IDbConnection cnn = new SqlConnection(sqlConn))
            {
                return cnn.ExecuteScalar<bool>(sql, new { data });
            }
        }
    }
}
