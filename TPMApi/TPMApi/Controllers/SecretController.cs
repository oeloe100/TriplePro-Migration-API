using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Authorize]
        public IActionResult Index()
        {
            var isSignedIn = _siginInManager.IsSignedIn(User);
            var IsAuthenticated = User.Identity.IsAuthenticated;

            return View();
        }
    }
}