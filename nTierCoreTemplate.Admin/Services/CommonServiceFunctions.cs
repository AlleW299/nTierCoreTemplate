using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{
    // System critical non-generated file, not subject to regeneration overwrites.
    public class CommonServiceFunctions
	{
        // Dependency injection - Repositories
        private readonly I_RoleRepository __RoleRepository;

        public CommonServiceFunctions(I_RoleRepository _RoleRepository)
        {
            __RoleRepository = _RoleRepository;
        }

        public async Task<IEnumerable<Claim>> GetUserClaims(_User user)
        {
            var roles = await Task.Run(() =>
            {
                return __RoleRepository.GetRoleNamesByUserId_Async(user._UserID);
            });

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName));
            claims.Add(new Claim("USERID", user._UserID));
            claims.Add(new Claim("EMAILID", user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims.AsEnumerable();
        }
    }
}
