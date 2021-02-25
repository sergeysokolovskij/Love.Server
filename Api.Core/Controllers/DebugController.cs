using Api.Core.Filters;
using Api.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class DebugController : BaseController
	{
		private readonly IHostingEnvironment hostingEnvironment;

		public DebugController(IServiceProvider serviceProvider,
			IUserProvider userProvider,
			IHostingEnvironment hostingEnvironment) : base(serviceProvider, userProvider)
		{
			this.hostingEnvironment = hostingEnvironment;
		}


		[PhoneConfirmed4AcessFilter]
		[HttpGet]
		public IActionResult Debug()
		{
			var buildDate = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
			var environment = hostingEnvironment.IsDevelopment() ? "Dev" : "Prod";
			
			return Json(new
			{
				isApiWork = true,
				buildDate = buildDate,
				host = Request.Host,
				environment = environment
			});
		}
	}
}
