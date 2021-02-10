using Api.Providers;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPlatforms.Core.Controllers
{
	[ApiController]
	public class UserController : BaseController
	{
		public UserController(IServiceProvider serviceProvider, 
			IUserProvider userProvider) : base(serviceProvider, userProvider)
		{
		}
	}
}
