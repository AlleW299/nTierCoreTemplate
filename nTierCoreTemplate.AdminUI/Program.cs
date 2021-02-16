using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nTierCoreTemplate.Core.Helpers;

namespace nTierCoreTemplate.AdminUI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>()
						.UseUrls("https://localhost:44335");
				})
				.ConfigureLogging((hostBuilderContext, logging) =>
				{
					logging.AddFileLogger(options => hostBuilderContext.Configuration.GetSection("Logging").GetSection("FileLogging").GetSection("Options").Bind(options));
				});
	}
}
