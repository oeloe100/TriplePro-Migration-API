using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TPMApi.Controllers
{
    public class SecretController : Controller
    {
        private readonly SignInManager<IdentityUser> _siginInManager;

        public SecretController(SignInManager<IdentityUser> siginInManager)
        {
            _siginInManager = siginInManager;
        }

        //[Authorize(AuthenticationSchemes = "JwtBearer")]
        [Authorize]
        public IActionResult Index()
        {
            var principal = User as ClaimsPrincipal;
            var check = User.Identity.IsAuthenticated;
            var isSignedIn = _siginInManager.IsSignedIn(User);

            return View();
        }
    }
}