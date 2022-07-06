using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;

namespace OnlineShop.AccountController
{
    public class ChangePasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ChangePasswordController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("account/changePassword")]

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("account/changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user,changePassword.OldPassword, changePassword.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            //_logger.LogInformation("User changed their password successfully.");
            //StatusMessage = "Your password has been changed.";

            return Redirect("/");
        }
    }
}
