using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Models;
using TPMDataLibrary.DataAccess;

namespace TPMDataLibrary.BusinessLogic
{
    public class WooDataProcessor
    {
        /// <summary>
        /// Insert data into database using dapper
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertAccessData(WooAccessModel model, string sqlConn)
        {
            var sql = @"INSERT INTO dbo.WooAccess (UserId, Name, WooClientKey, WooClientSecret, Created_At) VALUES 
                    (@UserId, @Name, @WooClientKey, @WooClientSecret, @Created_At);";

            return SQLDataAccess.SaveData(sql, model, sqlConn);
        }

        /// <summary>
        /// Check if record already exists
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool CompareWooSecretWithExistingRecords(string wooClientSecret, string sqlConn)
        {
            var sql = @"SELECT * FROM dbo.WooAccess WHERE WooClientSecret=@wooClientSecret";

            using (IDbConnection cnn = new SqlConnection(sqlConn))
            {
                return cnn.ExecuteScalar<bool>(sql, new { wooClientSecret });
            }
        }

        /// <summary>
        /// Get last record in WooAccess Table
        /// </summary>
        /// <returns></returns>
        public static List<WooAccessModel> GetLastAccessData(string sqlConn)
        {
            string sql = @"SELECT TOP 1 UserId, Name, WooClientKey, WooClientSecret, Created_At FROM dbo.WooAccess;";

            return SQLDataAccess.LoadData<WooAccessModel>(sql, sqlConn);
        }
    }
}
