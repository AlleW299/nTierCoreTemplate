using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using X.PagedList;
using nTierCoreTemplate.Core.Helpers;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;

namespace nTierCoreTemplate.Admin.Services
{
    // ----------------------------------------------------------------------------------------------------------
    // Machine read comment lines, Do not remove.
    // [ ] Check this box to prevent automatic overwrite when generating files.
    // Code within non commented region DMZ tags won't be overwritten by a system regeneration.
    // #region DMZ
    // #endregion DMZ 
    // Last created/Generated:  2020-02-21
    // Version:                 1.0.0.1
    // ----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Service for the managment of the _Setting table.
    ///   // Check if user is authenticated
	///	  if (!User.Identity.IsAuthenticated)
	///   {}
    /// </summary>
    public class _SettingService : I_SettingService
    {
        // Dependency injection - Repositories
        private readonly I_SettingRepository __SettingRepository;

        public _SettingService(IOptions<DataConnection> options)
        {
            var connection = options.Value;
            __SettingRepository = new _SettingRepository(connection.DefaultConnection);
        }

        #region DMZ - Default system functions.
        public async Task<List<_Setting>> GetAllSettingsByGroup_Async(string groupName)
        {
            var results = await Task.Run(() =>
            {
                return __SettingRepository.GetSetting_Special_Async(o => o.GroupName == groupName);
            });
            return results.ToList();
        }
        #endregion DMZ

        public async Task<X.PagedList.IPagedList<_Setting>> GetPagedList_Async(int? page)
        {
            var pageNumber = page ?? 1;
            var results = await Task.Run(() =>
            {
                return __SettingRepository.All_Async();
            });
            return results.ToPagedList(pageNumber, 10);
        }
    }
}
