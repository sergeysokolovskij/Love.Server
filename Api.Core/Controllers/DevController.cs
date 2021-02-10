using Api.Models.ResponseModel.DevController;
using Api.Provider;
using Api.Providers;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DevController : BaseController
	{
		private readonly IUserDevAccountProvider userDevAccountProvider; 

		public DevController(IServiceProvider serviceProvider, 
			IUserProvider userProvider,
			IUserDevAccountProvider userDevAccountProvider) : base(serviceProvider, userProvider)
		{
			this.userDevAccountProvider = userDevAccountProvider;
		}


		[HttpGet("devaccounts")]
		public async Task<IActionResult> GetDevUserAccountsAsync()
		{
			var userDevAccounts = await userDevAccountProvider.GetAllAsync();
			return Json(userDevAccounts.Select(x => new UserAccount()
			{
				Login = x.Login,
				Password = x.Password
			}).ToList());
		}
	}
}
