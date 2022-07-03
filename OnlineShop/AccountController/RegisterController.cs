using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineShop.Data;
using static Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal.ExternalLoginModel;
using Dapper;
using System.Threading.Tasks;
using OnlineShop.Models;
using System.Linq;

namespace OnlineShop.AccountController
{
    public class RegisterController : Controller
    { 
        private ApplicationDbContext _db;
        private IHostingEnvironment _he;
        private readonly IDbConnection _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        public RegisterController(ApplicationDbContext db, IHostingEnvironment he, IDbConnection configuration, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _db = db;
            _he = he;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        public string ReturnUrl { get; set; }

        [HttpGet]
        [Route("account/formodel")]
        public async Task<IActionResult> ForModelPopUp()

        {
            return View();
        }
        [HttpGet]
        [Route("account/register")]
        public async Task<IActionResult> Register()

        {
            return View();
        }

            [HttpPost]
        [Route("account/register")]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = register.Email,
                    Email = register.Email,
                    PhoneNumber = register.PhoneNumber,
                    FirstName = register.FirstName,
                    LastName = register.LastName

                };
                 var checkdublicate= await _configuration.QuerySingleOrDefaultAsync<IdentityUsers>("select Id from AspNetUsers where Email=@email", new {email=user.Email});
                if (checkdublicate.Id==null)
                {
                    var result = await _userManager.CreateAsync(user, register.Password);
                    var registeruser = await _configuration.QuerySingleOrDefaultAsync<IdentityUsers>("select Id from AspNetUsers where Email=@email", new { email = register.Email });
                    var userroleselect = await _configuration.QuerySingleOrDefaultAsync<Roles>("select * from AspNetRoles where Name=@name", new { name = "User" });
                    var user1 = _db.ApplicationUsers.FirstOrDefault(c => c.Id == registeruser.Id);
                    await _userManager.AddToRoleAsync(user1, userroleselect.Name);
                    if (result.Succeeded)
                    {

                        return Redirect("/account/formodel");
                    }
                 
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "UserName already Exists.");
                    return View(register);
                }
                
                return Redirect("/");
            }
            catch (System.Exception)
            {

                throw;
            }
           
           
        }
    }
}
