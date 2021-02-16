using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using nTierCoreTemplate.Core.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using nTierCoreTemplate.Admin.Models;

namespace nTierCoreTemplate.Admin
{
    [ApiController]
    public class _AuthController : Controller
    {
		// Dependency injections
		private readonly AppSetting _appSettings;
		private readonly DataConnection _dataConnection;
		// Dependency injection - Repositories
		private readonly I_UserRepository __UserRepository;
		private readonly I_RoleRepository __RoleRepository;

		public _AuthController(IOptions<AppSetting> appSettings,
							   IOptions<DataConnection> dataConnection,
							   I_UserRepository _UserRepository,
					           I_RoleRepository _RoleRepository)
		{
			_appSettings = appSettings.Value;
			_dataConnection = dataConnection.Value;
			__UserRepository = _UserRepository;
			__RoleRepository = _RoleRepository;


		}

		private bool AuthenticateJWT(string token)
        {
			return  new JwtSecurityTokenHandler().ReadToken(token).ValidTo < DateTime.UtcNow ? false : true;
        }

		private string CreateJWT(_User user)
        {
			var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_appSettings.Secret));
			var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				//new Claim(JwtRegisteredClaimNames.
				new Claim(JwtRegisteredClaimNames.Sub, user.Email), // this would be the username
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")) // this could a unique ID assigned to the user by a database
			};

			// Add Role permisions
			claims.AddRange(user.Roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r.Name)));

			var token = new JwtSecurityToken(
				issuer: _appSettings.ValidIssuer, 
				audience: _appSettings.ValidAudience, 
				claims: claims,
				expires: DateTime.Now.AddMinutes(_appSettings.SignInExpiresInMinutes), 
				signingCredentials: credentials);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[HttpPost]
		[Route("/api/auth/register")]
		public async Task<LoginResult> Post([FromBody] RegModel reg)
        {
			return new LoginResult { message = "User/password not found.", success = false };
		}

		[HttpPost]
		[Route("/api/auth/login")]
		public async Task<LoginResult> Post([FromBody] LoginModel log)
        {
			// Get user by username and password
			var user = await Task.Run(() =>
			{
				return new _UserRepository(_dataConnection.DefaultConnection).FindByUsernameAndHashedPassword_Async(log.username, log.password);
			});

			if (user != null)
			{
				// Get user by username and password
				var roles = await Task.Run(() =>
				{
					return new _RoleRepository(_dataConnection.DefaultConnection).GetRoleNamesByUserId_Async(user._UserID);
				});
				user.Roles = roles.ToList();

				return new LoginResult { message = "Login successful.", jwtBearer = CreateJWT(user), username = log.username, success = true };
			}
			
			return new LoginResult { message = "User/password not found.", success = false };
		}

		[HttpGet]
		[Route("/api/test")]
		public string TestApi()
        {
			return "Success";
		}

		[Authorize(Roles = "NonExistingRole")]
		[HttpGet]
		[Route("/api/roletest")]
		public string RoleTestApi()
		{
			return "Success";
		}
	}
}


//// Get User Claims
//var userClaims = await Task.Run(() =>
//{
//    return new CommonServiceFunctions(_roleRepository).GetUserClaims(user);
//});

////Authentication successful, Issue Token with user credentials
////Provides the security key which was given in the JWToken configuration in Startup.cs
//var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//// Generate Token for user
//// Rememebr to change issuer and audience to suit your environment.
//var JWToken = new JwtSecurityToken(
//    issuer: "http://localhost:44335/",
//    audience: "http://localhost:44335/",
//    claims: userClaims,
//    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
//    expires: new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToDouble(_appSettings.SignInExpiresInMinutes))).DateTime,
//    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //Using HS256 Algorithm to encrypt Token
//);
//var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
//return token;