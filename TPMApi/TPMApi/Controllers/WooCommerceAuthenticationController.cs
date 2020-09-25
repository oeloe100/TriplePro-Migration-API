using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TPMApi.Controllers
{
    [Authorize]
    public class WooCommerceAuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            return Ok();
        }
    }
}