using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

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
    /// Service for the managment of the _User table.
    ///   // Check if user is authenticated
    ///	  if (!User.Identity.IsAuthenticated)
    ///   {}
    /// </summary>
    public class _UserService : I_UserService //AuthenticationStateProvider, 
    {
        // Dependency injections
        private readonly IOptions<DataConnection> _dataConnection;
        private readonly ProtectedSessionStorage _protectedSessionStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Dependency injection repositories
        private readonly I_UserRepository __userRepository;
        //private readonly ILocalStorageService __localStorageService;

        private const string USER_SESSION_OBJECT_KEY = "user_session_obj";

        //public _UserService(ProtectedSessionStorage protectedSessionStore, IHttpContextAccessor httpContextAccessor) =>
        //        (this.protectedSessionStore, this.httpContextAccessor) = (protectedSessionStore, httpContextAccessor);

        public _UserService(IOptions<DataConnection> dataConnection, ProtectedSessionStorage protectedSessionStore, IHttpContextAccessor httpContextAccessor, I_UserRepository _userRepository)
        {
            _dataConnection = dataConnection;
            _protectedSessionStore = protectedSessionStore;
            _httpContextAccessor = httpContextAccessor;
           __userRepository = _userRepository;
        }


     //   public string IpAddress => httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;

        private _User User { get; set; }

        // var authState = await _UserService.GetAuthenticationStateAsync();
        // services.AddScoped<AuthenticationStateProvider, FakeAuthenticationStateProvider>();
        //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        //{
            //// read a possible user session object from the storage.
            //var user = await ILocalStorageService.GetItem<AuthenticationUser>("user");

            //if (user != null)
            //{
            //    //    return await GenerateAuthenticationState(userSession);
            //    //return await GenerateEmptyAuthenticationState();
            //var identity = new ClaimsIdentity();
            //foreach (var role in user.Roles)
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, role.NormalizedName));
            //}
            ////    var identity = new ClaimsIdentity(new[]
            ////    {
            ////            new Claim(ClaimTypes.Name, "Some fake user"),
            ////}, "Fake authentication type");

            //    var user = new ClaimsPrincipal(identity);
            //    return await Task.FromResult(new AuthenticationState(user));
            //}
         //   return await GenerateEmptyAuthenticationState();
       // }

        public async Task LoginAsync(_User user)
        {
            // store the session information in the client's storage.
            await SetUserSession(user);

            // notify the authentication state provider.
          //  NotifyAuthenticationStateChanged(GenerateAuthenticationState(user));
        }

        public async Task LogoutAsync()
        {
            // delete the user's session object.
            await SetUserSession(null);

            // notify the authentication state provider.
          //  NotifyAuthenticationStateChanged(GenerateEmptyAuthenticationState());
        }

        public async Task<_User> GetUserSession()
        {
            if (User != null)
                return User;

            string localUserJson = await _protectedSessionStore.GetAsync<string>(USER_SESSION_OBJECT_KEY);

            // no active user session found!
            if (string.IsNullOrEmpty(localUserJson))
                return null;

            try
            {
                return RefreshUserSession(JsonConvert.DeserializeObject<_User>(localUserJson));
            }
            catch
            {
                // user could have modified to local value, leading to an
                // invalid decrypted object. Hence, the user just did destory
                // his own session object. We need to clear it up.
                await LogoutAsync();
                return null;
            }
        }

        private async Task SetUserSession(_User user)
        {
            // buffer the current session into the user object,
            // in order to avoid fetching the user object from JS.
            RefreshUserSession(user);

            await _protectedSessionStore.SetAsync(USER_SESSION_OBJECT_KEY, JsonConvert.SerializeObject(user));
        }

        private _User RefreshUserSession(_User user) => User = user;

        //private Task<AuthenticationState> GenerateAuthenticationState(_User user)
        //{
        //    ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        //    {
        //        new Claim(ClaimTypes.Name, user.UserID.ToString()),
        //        new Claim(ClaimTypes.Role, user.UserName.ToString())
        //    }, "apiauth_type");

        //    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        //    return Task.FromResult(new AuthenticationState(claimsPrincipal));
        //}

        private Task<AuthenticationState> GenerateEmptyAuthenticationState() => Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        // ---------------------------------------------------------

        /// <summary>
        /// Service function for the _User Details API.
        /// </summary>
        public async Task<_User> Get_UserByEntityID_Async(string entityID)
        {
            return await Task.Run(() =>
            {
                return new _UserRepository(_dataConnection.Value.DefaultConnection).GetByEntityID_Async(entityID);
            });
        }


        public async Task<int> GetNumberOf_Users_Async()
        {
            return await Task.Run(() =>
            {
                return __userRepository.GetNumberOf_Users_Async();
            });
        }

        public async Task<X.PagedList.IPagedList<_User>> GetPagedList_Async(int? page)
        {
            var pageNumber = page ?? 1;
            var results = await Task.Run(() =>
            {
                return __userRepository.All_Async();
            });

            return results.ToPagedList(pageNumber, 10);
        }
    }
}
