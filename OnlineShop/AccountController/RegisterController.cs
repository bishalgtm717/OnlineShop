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
        //[BindProperty]
        //public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
        [HttpGet]
        [Route("Identity/Account/Register")]
        public async Task<IActionResult> Register()

        {
            return View();
        }

            [HttpPost]
        [Route("Identity/Account/Register")]
        public async Task<IActionResult> Register(Register register, string returnUrl = null)
        {
            var user = new ApplicationUser { UserName = register.Email,Email=register.Email   };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {

                return View(result);
            }
            return View("/");
        }
    }
}
