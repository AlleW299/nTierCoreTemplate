using nTierCoreTemplate.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{   // ----------------------------------------------------------------------------------------------------------
    // Machine read comment lines, Do not remove.
    // [ ] Check this box to prevent automatic overwrite when generating files.
    // Code within non commented region DMZ tags won't be overwritten by a system regeneration.
    // #region DMZ
    // #endregion DMZ 
    // Last created/Generated:  2020-02-21
    // Version:                 1.0.0.1
    // ----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Interface of the buisness logic service for all your _Role needs.
    /// </summary>
    public interface I_RoleService
    {
        #region DMZ - Default system functions.
        Task<IEnumerable<_Role>> GetRoleNamesByUserId_Async(string userId);
        #endregion

        Task<_Role> Get_RoleByEntityID_Async(string entityID);
        Task<int> GetNumberOf_Roles_Async();
        Task<_Role> Add_Role_Async(_Role entity);
        Task<X.PagedList.IPagedList<_Role>> GetPagedList_Async(int? page);
        Task DeleteByEntityID_Async(_Role entity);
    }
}
