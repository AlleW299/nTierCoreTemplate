using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using nTierCoreTemplate.Admin.Services;
using nTierCoreTemplate.Core.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using nTierCoreTemplate.Core.Entities;

namespace nTierCoreTemplate.Admin.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class _UserController : Controller
    {
        // Dependency injections
        private readonly IOptions<DataConnection> _dataConnection;
        // Dependency injection - Repositories
        private readonly _UserService __userService;
        private readonly _RoleService __roleService;
        private readonly IJSRuntime _js;


        public _UserController(IOptions<DataConnection> dataConnection, _UserService _userService, _RoleService _roleService, IJSRuntime js)
        {
            _dataConnection = dataConnection;
            __userService = _userService;
            __roleService = _roleService;
            _js = js;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [Authorize]
        [Route("api/_user/details/{entityID}")]
        public async Task<string> Details(string entityID)
        {
            var result = await Task.Run(() =>
            {
                var _user = __userService.Get_UserByEntityID_Async(entityID);
                return _user;
            });

            // Add user roles
            result.Roles = new List<_Role>();
            var roles = await Task.Run(() =>
            {
                var _role = __roleService.GetRoleNamesByUserId_Async(entityID);
                return _role;
            });
            if (roles != null)
                result.Roles.AddRange(roles);

            return System.Text.Json.JsonSerializer.Serialize(result);
        }
    }
}

////for serialization
//string serializedString = System.Text.Json.JsonSerializer.Serialize(User1);
////for deserialization
//User userCopy = System.Text.Json.JsonSerializer.Deserialize<User>(serializedString);

//await JSRuntime.InvokeAsync<object>("debugOut", (object)new Person[] {
//     new Person{FirstName=" Nancy",LastName=” Davolio”},
//      new Person{FirstName=" Andrew", LastName=” Fuller”}
//});

// 		return new LoginResult { message = "Login successful.", jwtBearer = CreateJWT(user), username = log.username, success = true };

