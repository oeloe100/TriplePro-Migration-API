using System;
using System.Collections.Generic;
using System.Text;
using TPMDataLibrary.DataAccess;
using TPMDataLibrary.Models;

namespace TPMDataLibrary.BusinessLogic
{
    public class TokenProcessor
    {
        /// <summary>
        /// Create SQL string with data from given model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertToken(AfostoAccessModel model)
        {
            string sql;

            sql = @"INSERT INTO dbo.AfostoAccess (AccessToken, RefreshToken, ExpiresIn) VALUES (@AccessToken, @RefreshToken, @ExpiresIn);";

            return SQLDataAccess.SaveData(
                "Server=(localdb)\\mssqllocaldb;Database=TPMApi;Trusted_Connection=True;MultipleActiveResultSets=true",
                sql, model);
        }
    }
}
