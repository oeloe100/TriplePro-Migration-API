using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TPMApi.Data.Context;
using TPMApi.Mapping.WTAMapping;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMDataLibrary.BusinessLogic;
using TPMDataLibrary.Models;

namespace TPMApi.Controllers
{
    [Authorize]
    public class WTAController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        /*private static WCObject _wcObject;*/
        private static WTAMapping _wtaMapping;

        public WTAController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
            _wtaMapping = new WTAMapping();
        }

        /// <summary>
        /// Create RedirectUri to Authenticate with Afosto API.
        /// Uri is being returned to clientside due to CORS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult Authenticate([FromForm]WTAAccessPoco model)
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
                        _config.Value.ConsumerKey,
                        _config.Value.CallbackUrl,
                        state.ToString());

                    /*
                    _wcObject = WooConnect.WcObject(
                        model.WooClientId,
                        model.WooClientSecret);
                        */

                    return Ok(authLocation);
                }
                catch (Exception ex)
                {
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
                    //Send appropriate data to afosto token endpoint and receive access data
                    var response = await AfostoAuthorizationMiddelware.AuthorizeClient(
                        _config.Value.AppServerUrl,
                        _config.Value.ConsumerKey,
                        _config.Value.ConsumerSecret,
                        _config.Value.CallbackUrl,
                        code);

                    //convert json string to jsonObject for several purposes
                    JObject jRespObject = JObject.Parse(response);
                    //conver jobject to pocoModel for readability and accessibility
                    AfostoTokensPoco tPoco = jRespObject.ToObject<AfostoTokensPoco>();

                    //Save accessdata to database.
                    TokenProcessor.InsertToken(new AfostoAccessModel
                    {
                        AccessToken = tPoco.AccessToken,
                        RefreshToken = tPoco.RefreshToken,
                        ExpiresIn = tPoco.ExpiresIn,
                    });

                    return Redirect("~/");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }

            return BadRequest();
        }

        /*
        private async Task<List<Product>> GetWCProducts(int page, int productPerPage)
        {
            var wcProducts = await _wcObject.Product.GetAll(new Dictionary<string, string>()
            {
                { "per_page", productPerPage.ToString()},
                { "page", page.ToString()}
            });

            return wcProducts;
        }
        */
    }
}
