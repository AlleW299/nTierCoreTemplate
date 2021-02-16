using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nTierCoreTemplate.Admin.Services
{
	// System critical non-generated file, not subject to regeneration overwrites.
	public interface ITokenProviderService
	{
		Task<string> LoginUser_Async(string userName, string password);
	}
}
