using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Helpers;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;

namespace nTierCoreTemplate.Admin.Services
{
    // System critical non-generated file, not subject to regeneration overwrites.
    public class APIAuthenticationService : IAPIAuthenticationService
    {
        // Dependency injections
        private readonly AppSetting _appSettings;
        // Dependency injection - Repositories
        private readonly I_UserRepository __UserRepository;
        private readonly I_RoleRepository __RoleRepository;

        public APIAuthenticationService(IOptions<AppSetting> appSettings, 
                                        I_UserRepository _UserRepository,
                                        I_RoleRepository _RoleRepository)
        {
            _appSettings = appSettings.Value;
            __UserRepository = _UserRepository;
            __RoleRepository = _RoleRepository;
        }

        /// <summary>
        /// Jwt authentication logic
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> Authenticate_Async(string userName, string password)
        {
            // Get user by username and password
            var user = await Task.Run(() =>
            {
                return __UserRepository.FindByUsernameAndHashedPassword_Async(userName, password);
            });

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            // Get User Claims
            var claimsIdentity = await Task.Run(() =>
            {
                var userClaims = Task.Run(() =>
                {
                    return new CommonServiceFunctions(__RoleRepository).GetUserClaims(user);
                });

                return new ClaimsIdentity(userClaims.Result);
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "http://localhost:44335/",
                Audience = "http://localhost:44335/",
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.SignInExpiresInMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var response = String.Format("{{ \"token\": \"{0}\" }}", tokenHandler.WriteToken(token));

            // Mod user object
            user.AuthorizationToken = tokenHandler.WriteToken(token);

           return response;
        }

    }
}
