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
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class MessangerContactsController : BaseController
	{
		public MessangerContactsController(IServiceProvider serviceProvider,
			IUserProvider userProvider) : base(serviceProvider, userProvider)
		{
		}


	}
}
