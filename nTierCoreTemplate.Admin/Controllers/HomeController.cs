using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using nTierCoreTemplate.Admin;
using nTierCoreTemplate.Core.CustomAttributes;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Admin.Services;
using nTierCoreTemplate.Core;

namespace nTierCoreTemplate.Admin.Controllers
{
	public class HomeController : Controller
	{
		// Dependency injections
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;
		private readonly ITokenProviderService _tokenProviderService;
		private readonly IStringLocalizer<Language> _sharedLocalizer;
		// Database tables
		private readonly I_SettingService __SettingService;
		private readonly I_UserService __UserService;
		private readonly I_PreRequsitsService __PreRequsitsService;

		protected readonly ILogger<HomeController> _logger;

		public HomeController(IHttpContextAccessor httpContextAccessor,
							  IConfiguration configuration,
							  ITokenProviderService tokenProviderService,
							  I_SettingService settingService,
							  I_UserService userService,
							  I_PreRequsitsService preRequsitsService,
							  IStringLocalizer<Language> sharedLocalizer,
							  [NotNull] ILogger<HomeController> logger
			)
		{
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
			_tokenProviderService = tokenProviderService;
			__SettingService = settingService;
			__UserService = userService;
			__PreRequsitsService = preRequsitsService;
			_sharedLocalizer = sharedLocalizer;

			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			_logger.LogInformation("Ran controller {controller}", "HomeController");

			// If there is no user account present, it creates a superadmin user with username "SuperAdmin" and password "sa299792!"..Remove after use.
			var allUsers = await Task.Run(() =>
			{
				return __UserService.GetNumberOf_Users_Async();
			});

			if (allUsers == 0)
			{
				//var task = Task.Run(async () => await new _PreRequsitsService(_tokenProviderService, _unitOfWork, _httpContextAccessor).RegisterSuperAdminAsync());
				var task = Task.Run(async () => await __PreRequsitsService.RegisterSuperAdminAsync());
				task.Wait();
			}

			// Check if user is authenticated
			if (!User.Identity.IsAuthenticated)
			{
				ViewBag.Authentication = "Superadmin account created. UserID: SuperAdmin - Password: sa299792!";
			}

			// Test Language
			ViewBag.LanguageTest = _sharedLocalizer["Language&CultureTest"];

			var actionLinkTask = Task.Run(async () => await __SettingService.GetAllSettingsByGroup_Async("ActionLink_Table")).Result;
			var buildStatusTask = Task.Run(async () => await __SettingService.GetAllSettingsByGroup_Async("Status")).Result;


			// Populate viewbag
			ViewBag.tableActionLinks = actionLinkTask;
			if (buildStatusTask != null)
			{
				ViewBag.GeneratedInSeconds = "- " + buildStatusTask.Where(w => w.Name == "BuildStatusInformation").FirstOrDefault().Value;
			}

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
