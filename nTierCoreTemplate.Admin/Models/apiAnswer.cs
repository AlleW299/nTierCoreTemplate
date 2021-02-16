using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Models
{
    public class ApiAnswer
    {
        public object data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}
