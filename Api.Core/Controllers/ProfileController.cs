using Api.Providers;
using Api.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Controllers
{
	[ApiController]
	[Authorize]
	[Route("[controller]")]
	public class ProfileController : BaseController
	{
		public ProfileController(IServiceProvider serviceProvider,
			IUserProvider userProvider) : base(serviceProvider, userProvider)
		{
		}


		[HttpGet]
		public async Task<IActionResult> IsProfileSucessAsync()
        {
			return Ok();
        }
	}
}
