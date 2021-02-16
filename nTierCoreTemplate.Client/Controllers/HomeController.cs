using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using nTierCoreTemplate.Core.CustomAttributes;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Client.Services;
using nTierCoreTemplate.Core;

namespace nTierCoreTemplate.Client.Controllers
{
	/// <summary>
	/// ClientUI - HomeController is a template controller and not subject to regeneration.
	/// After first use.. Pleaser remove all references and implementaions of the _PreRequsitsService and its interface, for security reasons.
	/// </summary>
	public class HomeController : Controller
	{
		// Dependency injections
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;
		private readonly ITokenProviderService _tokenProviderService;
		// Services
		private readonly I_UserService __userService;
		private readonly I_PreRequsitsService __preRequsitsService;

		public HomeController(IHttpContextAccessor httpContextAccessor, 
			                  IConfiguration configuration, 
							  ITokenProviderService tokenProviderService,
							  I_UserService _userService,
							  I_PreRequsitsService _preRequsitsService
			)
		{
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
			_tokenProviderService = tokenProviderService;
			__userService = _userService;
			__preRequsitsService = _preRequsitsService;
		}


		public async Task<IActionResult> Index()
		{
			// If there is no user account present, it creates a superadmin user with username "SuperAdmin" and password "sa299792!"..Remove after use.
			var users = await Task.Run(() =>
			{
				return __userService.GetNumberOf_Users_Async();
			});
				
			if (users == 0)
			{
				//var task = Task.Run(async () => await new _PreRequsitsService(_tokenProviderService, _unitOfWork, _httpContextAccessor).RegisterSuperAdminAsync());
				var task = Task.Run(async () => await __preRequsitsService.RegisterSuperAdminAsync());

				task.Wait();
			}

			// Check if user is authenticated
			if (!User.Identity.IsAuthenticated)
			{
				ViewBag.Authentication = "Superadmin account created. UserID: SuperAdmin - Password: sa299792!";
			}

			// Humber brag about duration of code generation process
			ViewBag.GeneratedInSeconds = "Generated in <GeneratedInSeconds /> seconds.";

			return View();
		}

		//[Authorize(Role.ANALYST)]
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
