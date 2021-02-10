using Api.Models.Options;
using Api.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Core.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DynamicController : BaseController
	{
		private readonly IOptions<TokenLifeTimeOptions> tokenOptions;

		public DynamicController(IServiceProvider serviceProvider, 
			IUserProvider userProvider,
			IOptions<TokenLifeTimeOptions> tokenOptions) : base(serviceProvider, userProvider)
		{
			this.tokenOptions = tokenOptions;
		}


		[HttpGet]
		public IActionResult GetDynamicConfig()
		{
			return Json(new
			{
				tokenOptions = new
				{
					acessLifeTime = tokenOptions.Value.AccessTokenLifeTime,
					refresh = tokenOptions.Value.RefreshTokenLifeTime
				}
			});
		}
	}
}
