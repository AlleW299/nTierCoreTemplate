using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core;
using nTierCoreTemplate.Client.Services;
using Microsoft.Extensions.Configuration;
using nTierCoreTemplate.Core.Helpers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Client.Controllers
{
    public class AccountController : Controller
    {
        // Dependency injections
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ITokenProviderService _tokenProviderService;

        public AccountController(ITokenProviderService tokenProviderService)
        {
            _tokenProviderService = tokenProviderService;
        }

        public IActionResult Login(AccountViewModel_LoginUser user)
        {
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> LoginUser(AccountViewModel_LoginUser user)
        {
            //Authenticate user
            var userToken = await Task.Run(() =>
            {
                return _tokenProviderService.LoginUser_Async(user.Username.Trim(), user.Password.Trim());
            });

                
            if (userToken != null)
            {
                //Save token in session object
                HttpContext.Session.SetString("JWToken", userToken);

                return Redirect("~/Home/Index");
            }

            // In case of error
            user.AddError("Invalid username or password.");
            return View("Login", user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Index");
        }
    }
}