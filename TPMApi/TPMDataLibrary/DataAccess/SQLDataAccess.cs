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
        //remove hardcoded part. get it from appsettings.json TODO
        public static string _conn = "Server=(localdb)\\mssqllocaldb;Database=TPMApi;Trusted_Connection=True;MultipleActiveResultSets=true";

        /// <summary>
        /// Save data to database using DAPPER
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int SaveData<T>(string sql, T data)
        {
            using (IDbConnection cnn = new SqlConnection(_conn))
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
        public static List<T> LoadData<T>(string sql)
        {
            using (IDbConnection cnn = new SqlConnection(_conn))
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
        public static bool CompareData(string sql, string data)
        {
            using (IDbConnection cnn = new SqlConnection(_conn))
            {
                return cnn.ExecuteScalar<bool>(sql, new { data });
            }
        }
    }
}
