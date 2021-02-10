using Api.DAL;
using Api.Providers;
using Api.Services.Cache;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopPlatform.Controllers
{
	public class BaseController : Controller
	{
		protected readonly IServiceProvider serviceProvider;
		protected readonly IUserProvider userProvider;

		public BaseController(IServiceProvider serviceProvider,
			IUserProvider userProvider)
		{
			this.serviceProvider = serviceProvider;
			this.userProvider = userProvider;
		}

		public BaseController(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		

		[NonAction]
		public IActionResult JsonMessage(string message)
		{
			return Json(new
			{
				message = message
			});
		}

		[NonAction]
		public IActionResult JsonExceptionMessage(string message)
		{
			Response.ContentType = "application/json";
			return StatusCode(400, new
			{
				exception = message
			});
		}

		[NonAction]
		public IActionResult Forbidden()
		{
			return StatusCode(403);
		}

		[NonAction]
		public IActionResult Updated()
		{
			return StatusCode(204);
		}

		[NonAction]
		public IActionResult Created(object obj = null)
		{
			if (obj != null)
				return StatusCode(201, obj);
			return StatusCode(201);
		}

		

		[NonAction]
		public Task<User> GetUserAsync()
		{
			var allRoles = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
			if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
				return userProvider.GetModelBySearchPredicate(x => x.UserName == User.Identity.Name);
			return null;
		}

		public string ControllerName => ControllerContext.ActionDescriptor.ControllerName;
		public string ActionName => ControllerContext.ActionDescriptor.ActionName;

		public string UserName
		{
			get
			{
				return User.Identity.Name;
			}
		}

		public List<string> RoleNames
		{
			get
			{
				return User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
			}
		}

		public string TokenId
		{
			get
			{
				return User.Claims.Where(x => x.Type == CommonConstants.UniqueClaimName).Select(x => x.Value).FirstOrDefault();
			}
		}
	}
}
