using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TPMApi.Models;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;
using WooCommerceNET.WooCommerce.v3;
using WooCommerceNET.WooCommerce.v3.Extension;

namespace TPMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WooController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public WooController(
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // POST: api/Woo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromForm] WooAccessFormPoco form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RestAPI restApi = new RestAPI("https://www.hetsteigerhouthuis.nl/wp-json/wc/v3/",
                        form.ClientKey,
                        form.ClientSecret,
                        requestFilter : RequestFilter);

                    WCObject wcObject = new WCObject(restApi);

                    var products = await wcObject.Product.GetAll(new Dictionary<string, string>() 
                    {
                        { "per_page", "50" }
                    });

                    return Ok(products);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    return BadRequest(ex);
                }
            }

            return BadRequest("Modelstate is not valid");
        }

        private void RequestFilter(HttpWebRequest request)
        {
            request.UserAgent = "Woocommerce.NET";
        }
    }
}
