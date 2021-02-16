using Microsoft.AspNetCore.Components.Authorization;
using nTierCoreTemplate.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    /// Interface of the buisness logic service for all your _User needs.
    /// </summary>
    public interface I_UserService
    {
        //Task<AuthenticationState> GetAuthenticationStateAsync();
        Task<int> GetNumberOf_Users_Async();
        Task<_User> Get_UserByEntityID_Async(string entityID);
        Task<X.PagedList.IPagedList<_User>> GetPagedList_Async(int? page);
    }
}
