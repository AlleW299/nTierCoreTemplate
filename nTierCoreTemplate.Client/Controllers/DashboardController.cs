using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nTierCoreTemplate.Core.CustomAttributes;
using nTierCoreTemplate.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Client.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult DirectorPage()
        {
            return View("DirectorPage");
        }

        public IActionResult SupervisorPage()
        {
            ViewBag.Message = "Permission controlled through action Attribute";
            return View("SupervisorPage");
        }

       // [Authorize(Role.ANALYST)]
        public IActionResult AnalystPage()
        {
            return View("AnalystPage");
        }

        public IActionResult SupervisorAnalystPage()
        {
            ViewBag.Message = "Permission controlled inside action method";
            //if (this.HavePermission(Role.SUPERVISOR))
            //    return View("SupervisorPage");

            //if (this.HavePermission(Role.ANALYST))
            //    return View("AnalystPage");

            return new RedirectResult("~/Dashboard/NoPermission");
        }

    }

}
