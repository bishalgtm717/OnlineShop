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
using System.Data;
using Dapper;


namespace OnlineShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _db;
        private readonly IDbConnection _configuration;

        public LoginController(SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager, ApplicationDbContext db, IDbConnection configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager; 
            _db = db;
            _configuration = configuration;
        }
        [Route("account/login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }


        [HttpPost]
        [Route("account/login")]
        public async Task<IActionResult> Login(Login login, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            { 
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
                    return LocalRedirect(returnUrl);
                }
                
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    ModelState.AddModelError("Password", "Incorrect Password.");
                    return View(login);
                }
            }

            // If we got this far, something failed, redisplay form
            return Redirect("/");
        }
    }

} 