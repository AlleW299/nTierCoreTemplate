using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using nTierCoreTemplate.Core.Entities;
using nTierCoreTemplate.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{
    // System critical non-generated file, not subject to regeneration overwrites.
    public interface IAPIAuthenticationService
    {
        Task<string> Authenticate_Async(string userName,string password);
    }
}
