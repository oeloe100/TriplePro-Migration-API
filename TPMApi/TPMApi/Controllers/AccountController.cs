using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TPMApi.Models;

namespace TPMApi.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signinManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signinManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EmailConfirmation()
        {
            return View();
        }

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

        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return Redirect("~/");
        }
    }
}