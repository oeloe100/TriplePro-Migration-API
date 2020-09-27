using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPMApi.Models;
using TPMApi.Services;
using TPMDataLibrary.BusinessLogic;

namespace TPMApi.Controllers
{
    [Authorize]
    public class WooCommerceAuthenticationController : Controller
    {
        private static UserManager<IdentityUser> _userManager;

        public WooCommerceAuthenticationController(
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Authenticate(WooAccessPoco model)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var containsRecords = WooDataProcessor.CompareWooSecretWithExistingRecords(
                        model.WooClientSecret);

                if (!containsRecords)
                {
                    //insert woocommerce data in to database
                    WooAccessModel wooAccessModel = new WooAccessModel()
                    {
                        UserId = user.Id,
                        WooClientKey = model.WooClientId,
                        WooClientSecret = model.WooClientSecret,
                        Created_At = DateTime.UtcNow,
                        Name = model.Name,
                    };

                    //Insert datamodel into database
                    WooDataProcessor.InsertAccessData(wooAccessModel);
                }

                return Ok("Https://" + this.Request.Host + "/Migration/Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}