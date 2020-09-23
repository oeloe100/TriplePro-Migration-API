using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.Services;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Controllers
{
    [Authorize]
    public class AfostoAuthorizationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        private static WCObject _wcObject;

        public AfostoAuthorizationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
        }

        public IActionResult Authenticate([FromForm]WTAAccessPoco model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //*** DONT SAVE ANYWHERE ***//
                    var state = Guid.NewGuid();
                    HttpContext.Session.SetString("state", state.ToString());

                    var authLocation = AfostoAuthorizationMiddelware.RequestAuthorizationUrl(
                        _config.Value.ServerUrl,
                        _config.Value.ConsumerKey,
                        _config.Value.CallbackUrl,
                        state.ToString());

                    _wcObject = WooConnect.WcObject(
                        model.WooClientId, 
                        model.WooClientSecret);

                    return Ok(authLocation);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message + ex.StackTrace);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code, string state)
        {
            if (state == HttpContext.Session.GetString("state"))
            {
                try
                {
                    var response = await AfostoAuthorizationMiddelware.AuthorizeClient(
                        _config.Value.ServerUrl,
                        _config.Value.ConsumerKey,
                        _config.Value.ConsumerSecret,
                        _config.Value.CallbackUrl,
                        code);

                    JObject jRespObject = JObject.Parse(response);
                    AfostoTokensPoco tPoco = jRespObject.ToObject<AfostoTokensPoco>();

                    var test = _wcObject;

                    return Ok(tPoco);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }

            return BadRequest();
        }
    }
}