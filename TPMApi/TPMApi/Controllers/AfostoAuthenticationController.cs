using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMDataLibrary.BusinessLogic;
using TPMDataLibrary.Models;

namespace TPMApi.Controllers
{
    [Authorize]
    public class AfostoAuthenticationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        private static UserManager<IdentityUser> _userManager;
        private readonly ILogger<AfostoAuthenticationController> _logger;

        private SqlConnection _sqlConn;

        public AfostoAuthenticationController(
            IOptions<AuthorizationPoco> config,
            UserManager<IdentityUser> userManager,
            ILogger<AfostoAuthenticationController> logger)
        {
            _config = config;
            _userManager = userManager;
            _logger = logger;

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            SqlConnection sqlConn = new SqlConnection(builder.
                        GetSection("ConnectionStrings").
                        GetSection("TPMApiContextConnection").Value);

            _sqlConn = sqlConn;
        }

        /// <summary>
        /// Create RedirectUri to Authenticate with Afosto API.
        /// Uri is being returned to clientside due to CORS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Authenticate([FromForm] AfostoAccessPoco model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //*** DONT SAVE ANYWHERE ***//
                    var state = Guid.NewGuid();
                    //Save state in memory we verify this in the callback
                    HttpContext.Session.SetString("state", state.ToString());

                    //build Authorization Uri
                    var authLocation = AfostoAuthorizationMiddelware.RequestAuthorizationUrl(
                        _config.Value.AppServerUrl,
                        model.AfostoClientId,
                        _config.Value.CallbackUrl,
                        state.ToString());

                    var user = await _userManager.GetUserAsync(User);

                    //insert first afosto access data into db.
                    AfostoAccessModel afostoAccessModel = new AfostoAccessModel()
                    {
                        UserId = user.Id,
                        AfostoKey = model.AfostoClientId,
                        AfostoSecret = model.AfostoClientSecret,
                        Created_At = DateTime.UtcNow,
                        Name = model.Name,
                    };

                    //Insert datamodel into database
                    AfostoDataProcessor.InsertAccessData(afostoAccessModel, _sqlConn.ConnectionString);

                    return Ok(authLocation);

                    //We return the URL for WooCommerce Authentication
                    //return Ok($"{ this.Request.Scheme }://{ this.Request.Host }/WooCommerceAuthentication/Index");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    return BadRequest(ex.Message + ex.StackTrace);
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Send necessary data to afosto token endpoint and receive accesstoken
        /// Save received data to server so we can use it later. Seems to work better dan direct data.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Callback(string code, string state)
        {
            //compare state with in memory to avoid cross forgery
            if (state == HttpContext.Session.GetString("state"))
            {
                try
                {
                    //Load afosto access model from SQL DB
                    var AfostoAccessModel = AfostoDataProcessor.GetAccessData(_sqlConn.ConnectionString);

                    //Send appropriate data to afosto token endpoint and receive access data
                    var response = await AfostoAuthorizationMiddelware.AuthorizeClient(
                        _config.Value.AppServerUrl,
                        AfostoAccessModel[AfostoAccessModel.Count - 1].AfostoKey,
                        AfostoAccessModel[AfostoAccessModel.Count - 1].AfostoSecret,
                        _config.Value.CallbackUrl,
                        code);

                    //convert json string to jsonObject for several purposes
                    JObject jRespObject = JObject.Parse(response);
                    //conver jobject to pocoModel for readability and accessibility
                    AfostoTokensPoco tPoco = jRespObject.ToObject<AfostoTokensPoco>();

                    //Save access/refresh token to database. Db is being altered.
                    AfostoDataProcessor.EditCallbackAccessData(new AfostoAccessModel
                    {
                        AccessToken = tPoco.AccessToken,
                        RefreshToken = tPoco.RefreshToken,
                        ExpiresIn = tPoco.ExpiresIn,
                    }, _sqlConn.ConnectionString);

                    return RedirectToRoute(new { action = "Index", controller = "WooCommerceAuthentication" });
                    //$"{ this.Request.Scheme }://{ this.Request.Host }/WooCommerceAuthentication/Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    return BadRequest(ex);
                }
            }

            return BadRequest();
        }
    }
}
