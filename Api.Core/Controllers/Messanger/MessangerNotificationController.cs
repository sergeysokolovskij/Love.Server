using Api.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Controllers.Messanger
{
	[Route("[controller]")]
	[ApiController]
	public class MessangerNotificationController : BaseController
	{
		public MessangerNotificationController(IServiceProvider serviceProvider, 
			IUserProvider userProvider) : base(serviceProvider, userProvider)
		{
		}

		[HttpGet("getdialogs")]
		[Authorize(Roles = "ProtocoledUsers")]
		public async Task<IActionResult> GetDialogListAsync()
		{
			var user = await GetUserAsync();
			return Ok();
		}
	}
}
