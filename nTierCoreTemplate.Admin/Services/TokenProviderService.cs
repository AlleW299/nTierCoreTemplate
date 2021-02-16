using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using nTierCoreTemplate.Core.Helpers;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{
    // System critical non-generated file, not subject to regeneration overwrites.
    public class TokenProviderService : ITokenProviderService
	{
        // Dependency injections
        private readonly AppSetting _appSettings;
        // Dependency injection - Repositories
        private readonly I_UserRepository _userRepository;
        private readonly I_RoleRepository _roleRepository;

        public TokenProviderService(IOptions<AppSetting> appSetting, IOptions<DataConnection> options)
        {
            var connection = options.Value;
            _appSettings = appSetting.Value;
            _userRepository = new _UserRepository(connection.DefaultConnection);
            _roleRepository = new _RoleRepository(connection.DefaultConnection);
        }

        public async Task<string> LoginUser_Async(string userName, string password)
        {
            //Get user object for the user who is trying to login
            var user = await Task.Run(() =>
            {
                return _userRepository.FindByUsernameAndHashedPassword_Async(userName, password);
            });

            // return null if user not found or invalid
            if (user == null)
                return null;

            // Get User Claims
            var userClaims = await Task.Run(() =>
            {
                return new CommonServiceFunctions(_roleRepository).GetUserClaims(user);
            });

            //Authentication successful, Issue Token with user credentials
            //Provides the security key which was given in the JWToken configuration in Startup.cs
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            // Generate Token for user
            // Rememebr to change issuer and audience to suit your environment.
            var JWToken = new JwtSecurityToken(
                issuer: "http://localhost:44335/",
                audience: "http://localhost:44335/",
                claims: userClaims,
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToDouble(_appSettings.SignInExpiresInMinutes))).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //Using HS256 Algorithm to encrypt Token
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;

        }
    }
}
