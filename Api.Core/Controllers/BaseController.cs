using Api.Services.Cache;
using Api.Services.Cache.CategoryTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopPlatform.Controllers
{
	public class BaseController : Controller
	{
		protected readonly ICacheService cacheService;
		protected readonly IServiceProvider serviceProvider;

		public BaseController(IServiceProvider serviceProvider)
		{
		}

		public BaseController(IServiceProvider serviceProvider,
			ICacheService cacheService)
		{
			this.serviceProvider = serviceProvider;
			this.cacheService = cacheService;
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

		//Реализация кеш-логики здесь. Её нужно будет обдумать 
		[NonAction]
		public async Task <IActionResult> CacheContentAsync<T>(
			string cacheKey,
			Func<Task<T>> dataLoader,
			ITypeCache categoryCache)
		{
			var result = cacheService.Get(cacheKey);
			if (result == null)
			{
				result = await dataLoader();
				cacheService.Set(cacheKey, result, categoryCache);
			}
			if (result == null)
				return Ok();
			return Json(result);
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
		public string RoleName
		{
			get
			{
				return User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
			}
		}
	}
}
