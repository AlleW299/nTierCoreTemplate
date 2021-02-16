using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using nTierCoreTemplate.Client.Services;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core;
using System.Linq;
using System.Security.Claims;

namespace nTierCoreTemplate.Client.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class APIAuthenticationController : ControllerBase
    {
        // Dependency injections
        private IAPIAuthenticationService _authenticationService;

        public APIAuthenticationController(IAPIAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Api Athentication
        /// Accessable through 'https://localhost:44335/APIAuthenticationController/authenticate'
        /// {
        ///     username: "",
        ///     password: ""
        /// }
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody]AuthenticateModel model)
        {
            var response = _authenticationService.Authenticate_Async(model.Username, model.Password);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        /// <summary>
        /// Testing API, might want to disable before launch
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("GetAllRolesOfUser")]
        public IActionResult GetAllRolesOfUser()
        {
            return Ok(User.Claims.Where(o => o.Type == ClaimTypes.Role).Select(c =>
                   new
                   {
                       Role = c.Value
                   }).OrderBy(o=> o.Role));
        }
    }
}
