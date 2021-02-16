using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Client.Services
{
	public interface I_PreRequsitsService
	{
		Task<string> RegisterSuperAdminAsync();
	}
}
