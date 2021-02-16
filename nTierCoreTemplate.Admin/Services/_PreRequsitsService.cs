using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Helpers;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{
	// ----------------------------------------------------------------------------------------------------------
	// Machine read comment lines, Do not remove.
	// [ ] Check this box to prevent automatic overwrite when generating files.
	// Code within #region DMZ and #endregion DMZ tags won't be overwritten by a system regeneration.
	// Last created/Generated:  2020-02-21
	// Version:                 1.0.0.1
	// ----------------------------------------------------------------------------------------------------------
	/// <summary>
	/// ToDo - Remove _PreRequsitsService and I_PreRequsitsService after use...
	/// </summary>
	public class _PreRequsitsService : I_PreRequsitsService
	{
		// Dependency injections
		private readonly ITokenProviderService _tokenProviderService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		// Dependency injection repositories
		private readonly I_UserRepository _userRepository;
		private readonly I_RoleRepository _roleRepository;
		private readonly I_UserRoleRepository _userRoleRepository;

		/// <summary>
		/// Buisness logic service for all your _User needs.
		/// </summary>
		public _PreRequsitsService(IOptions<DataConnection> options)
		{
			var connection = options.Value;
			_userRepository = new _UserRepository(connection.DefaultConnection);
			_roleRepository = new _RoleRepository(connection.DefaultConnection);
			_userRoleRepository = new _UserRoleRepository(connection.DefaultConnection);
			//_tokenProviderServic = new TokenProviderService(appSettings, options);
		}

		/// <summary>
		/// Generates a superAdmin account. Remove function after first use.
		/// Remember to change Username and/or Password after initialization.
		/// </summary>
		/// <returns></returns>
		public async Task<string> RegisterSuperAdminAsync()
		{
			// Create SuperAdmin Role
			var superAdminRole = new _Role();
			superAdminRole.Name = "SuperAdmin";
			var newRole = await Task.Run(() =>
			{
				var results = _roleRepository.Add_Async(superAdminRole);
				return results;
			});

			//var roleCreation = await _roleStore.CreateAsync(superAdminRole, new CancellationToken());
			if (newRole != null)
			{
				// Create SuperAdmin User
				var password = "sa299792!";
				var appUser = new _User
				{
					Name = "SuperAdmin",
					FirstName = "SuperAdmin",
					LastName = " ",
					UserName = "SuperAdmin",
					SocSecNr ="11111111-1111",
					NormalizedUserName = "SUPERADMIN",
					Email = "Alexanderwahlin299@gmail.com",
					NormalizedEmail = "ALEXANDERWAHLIN299@GMAIL.COM",
					PasswordHash = Convert.ToBase64String(Encoding.ASCII.GetBytes(password)),
					PhoneNumber = "555-12345",
					LockoutEnd = Convert.ToDateTime("1900-01-01 00:00:01"),
					SecurityStamp = Guid.NewGuid().ToString(),
					Enabled = true,
					Notes="Delete, change or disable before going live.",
					Password=""
				};
				var newUser = await Task.Run(() =>
				{
					return  _userRepository.Add_Async(appUser, 1);
				});

				// Add user to Superadmin role.
				var newUserRole = await Task.Run(() =>
				{
					var newUserRole = new _UserRole { UserId = ((_User)newUser)._UserID, RoleId = ((_Role)newRole)._RoleID };
					return _userRoleRepository.Add_Async(newUserRole,1);
				});

				//Authenticate newly created user.
				//return await Task.Run(() =>
				//{
				//	return _tokenProviderService.LoginUser_Async(appUser.UserName, password);
				//});
			}

			return "";
		}
	}
}