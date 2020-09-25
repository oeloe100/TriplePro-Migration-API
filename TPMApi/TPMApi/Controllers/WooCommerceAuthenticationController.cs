using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TPMApi.Controllers
{
    public class WooCommerceAuthenticationController : Controller
    {
        public IActionResult Authenticate()
        {
            return Ok();
        }
    }
}