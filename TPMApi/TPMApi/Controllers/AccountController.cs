using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TPMApi.Models;

namespace TPMApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signinManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EmailConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Simple user login using .NET CORE Identity.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<IActionResult> Login(LoginPoco login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                var passCheck = await _userManager.CheckPasswordAsync(user, login.Password);

                if (passCheck && user != null)
                {
                    var canSignIn = await _signinManager.CanSignInAsync(user);
                    if (canSignIn)
                    {
                        await _signinManager.SignInAsync(user, false);
                        return Redirect("~/");
                    }
                }
            }

            return Redirect("~/");
        }

        /// <summary>
        /// Simple logout using .NET Core Identity
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return Redirect("~/");
        }
    }
}