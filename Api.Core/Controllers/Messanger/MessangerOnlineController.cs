using Api.Providers;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Core.Controllers.Messanger
{
	[Route("[controller]")]
	[ApiController]
	public class MessangerOnlineController : BaseController
	{
		public MessangerOnlineController(IServiceProvider serviceProvider, 
			IUserProvider userProvider) : base(serviceProvider, userProvider)
		{
		}
	}
}
