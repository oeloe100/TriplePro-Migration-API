using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Models;
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
        public static int InsertAccessData(AfostoAccessModel model)
        {
            var sql = @"INSERT INTO dbo.AfostoAccess (AccessToken, RefreshToken, ExpiresIn, Created_At, UserId, Name, AfostoKey, AfostoSecret) VALUES 
                    (@AccessToken, @RefreshToken, @ExpiresIn, @Created_At, @UserId, @Name, @AfostoKey, @AfostoSecret);";

            return SQLDataAccess.SaveData(sql, model);
        }

        /// <summary>
        /// Check if record already exists
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool CompareAfostoSecretWithExistingRecords(string afostoSecret)
        {
            var sql = @"SELECT * FROM dbo.AfostoAccess WHERE AfostoSecret=@afostoSecret";

            using (IDbConnection cnn = new SqlConnection(SQLDataAccess._conn))
            {
                return cnn.ExecuteScalar<bool>(sql, new { afostoSecret });
            }
        }

        /// <summary>
        /// Update specifi Authentication Sql data from given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int EditCallbackAccessData(AfostoAccessModel model)
        {
            var sql = @"Update dbo.AfostoAccess SET AccessToken=@AccessToken, RefreshToken=@RefreshToken, ExpiresIn=@ExpiresIn";

            return SQLDataAccess.SaveData(sql, model);
        }
    }
}
