using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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
        public static int SaveData<T>(string connectionString, string sql, T data)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
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
        public static List<T> LoadData<T>(string connectionString, string sql)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }
    }
}
