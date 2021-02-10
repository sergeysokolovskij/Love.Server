using Api.Providers;
using Api.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
