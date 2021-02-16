using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace nTierCoreTemplate.Admin.Services
{    public class _RoleService : I_RoleService
    {
        // Dependency injections
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly I_RoleRepository __RoleRepository;

        public _RoleService(IOptions<DataConnection> options, IHttpContextAccessor httpContextAccessor)
        {
            var connection = options.Value;
            _httpContextAccessor = httpContextAccessor;
            __RoleRepository = new _RoleRepository(connection.DefaultConnection);
        }

        #region DMZ - Default system functions.
        public async Task<IEnumerable<_Role>> GetRoleNamesByUserId_Async(string userId)
        {
            return await Task.Run(() =>
            {
                return __RoleRepository.GetRoleNamesByUserId_Async(userId);
            });
        }
        #endregion DMZ

        /// <summary>
        /// Service function for the _Role Details View.
        /// </summary>
        public async Task<_Role> Get_RoleByEntityID_Async(string entityID)
        {
            return await Task.Run(() =>
            {
                return __RoleRepository.GetByEntityID_Async(entityID);
            });
        }

        public async Task<int> GetNumberOf_Roles_Async()
        {
            return await Task.Run(() =>
            {
                return __RoleRepository.GetNumberOf_Roles_Async();
            });
        }

        public async Task<X.PagedList.IPagedList<_Role>> GetPagedList_Async(int? page)
        {
            var pageNumber = page ?? 1;
            var results = await Task.Run(() =>
            {
                return __RoleRepository.All_Async();
            });

            return results.ToPagedList(pageNumber, 10);
        }

        /// <summary>
        /// Service function for the _Role Add View.
        /// </summary>
        public async Task<_Role> Add_Role_Async(_Role entity)
        {
            return await Task.Run(() =>
            {
                return __RoleRepository.Add_Async(entity, null);
            });
        }

        /// <summary>
        /// Service function for the delete _Role.
        /// </summary>
        public async Task DeleteByEntityID_Async(_Role entity)
        {
            // Check if user is authenticated
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                await Task.Run(() =>
                {
                    __RoleRepository.DeleteByEntityID_Async(entity._RoleID);
                });
            }
        }
    }
}
