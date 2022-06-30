using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;
using OnlineShop.Models;


namespace OnlineShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _db;


        public LoginController(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }

            public async Task<IActionResult> Login(Login login, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.UserName.ToLower() == login.Email.ToLower());
                    var roleInfo = (from ur in _db.UserRoles
                                    join r in _db.Roles on ur.RoleId equals r.Id
                                    where ur.UserId == userInfo.Id
                                    select new SessionUserVm()
                                    {
                                        UserName = login.Email,
                                        RoleName = r.Name
                                    }).FirstOrDefault();
                    if (roleInfo != null)
                    {
                        HttpContext.Session.SetString("roleName", roleInfo.RoleName);
                    }
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = login.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Redirect("/");
                }
            }

            // If we got this far, something failed, redisplay form
            return Redirect("/");
        }
    }

}