using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TPMDataLibrary.DataAccess;
using TPMDataLibrary.Models;

namespace TPMDataLibrary.BusinessLogic
{
    public class AfostoDataProcessor
    {
        /// <summary>
        /// Create SQL string with data from given model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertAccessData(AfostoAccessModel model, string sqlConn)
        {
            var sql = @"INSERT INTO dbo.AfostoAccess (AccessToken, RefreshToken, ExpiresIn, Created_At, UserId, Name, AfostoKey, AfostoSecret) VALUES 
                    (@AccessToken, @RefreshToken, @ExpiresIn, @Created_At, @UserId, @Name, @AfostoKey, @AfostoSecret);";

            return SQLDataAccess.SaveData(sql, model, sqlConn);
        }

        /// <summary>
        /// Get last record in AfostoAccess Table
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLastAccessToken(string sqlConn)
        {
            string sql = @"SELECT TOP 1 AccessToken FROM dbo.AfostoAccess;";

            return SQLDataAccess.LoadData<string>(sql, sqlConn);
        }

        /// <summary>
        /// Check if record already exists
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool CompareAfostoSecretWithExistingRecords(string afostoSecret, string sqlConn)
        {
            var sql = @"SELECT * FROM dbo.AfostoAccess WHERE AfostoSecret=@afostoSecret";

            using (IDbConnection cnn = new SqlConnection(sqlConn))
            {
                return cnn.ExecuteScalar<bool>(sql, new { afostoSecret });
            }
        }

        public static List<AfostoAccessModel> GetAccessData(string sqlConn)
        {
            string sql = @"SELECT * FROM dbo.AfostoAccess;";

            return SQLDataAccess.LoadData<AfostoAccessModel>(sql, sqlConn);
        }

        /// <summary>
        /// Update specifi Authentication Sql data from given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int EditCallbackAccessData(AfostoAccessModel model, string sqlConn)
        {
            var sql = @"Update dbo.AfostoAccess SET AccessToken=@AccessToken, RefreshToken=@RefreshToken, ExpiresIn=@ExpiresIn";

            return SQLDataAccess.SaveData(sql, model, sqlConn);
        }
    }
}
