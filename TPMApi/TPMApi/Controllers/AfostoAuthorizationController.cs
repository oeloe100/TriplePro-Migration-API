using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TPMApi.Helpers;
using TPMApi.Middelware;
using TPMApi.Models;

namespace TPMApi.Controllers
{
    public class AfostoAuthorizationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;

        public AfostoAuthorizationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
        }

        public IActionResult Authorize([FromForm]UserAccessPoco pocoModel)
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

                    return Redirect(authLocation);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message + ex.StackTrace);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult ReceiveAccessToken(string code, string state)
        {
            throw new NotImplementedException();
        }
    }
}