using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShopPlatforms.Core.Controllers
{
	public class CaptchaController : BaseController
	{
		public CaptchaController(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
	}
}
