using Api.Models.RequestModels.MessangerSessionController;
using Api.Providers;
using Api.Services.Cache;
using Api.Services.Messanger;
using Api.Services.Processing;
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
	[Authorize]
	public class MessangerSessionController : BaseController
	{
		private readonly ISessionService sessionService;
	
		public MessangerSessionController(IServiceProvider serviceProvider, 
			IUserProvider userProvider,
			ISessionService sessionService) : base(serviceProvider, userProvider)
		{
			this.sessionService = sessionService;
		}

		
		[Authorize(Roles = "User")]
		[HttpPost("createfirstsession")]
		public async Task<IActionResult> CreateFirstSessionAsync([FromBody]CreateMessangerSessionRequest model)
		{
			var user = await GetUserAsync();
			var result = await sessionService.MakeFirstSessionAsync(model, user.Id, TokenId);

			return Json(result);
		}


		[Authorize(Roles = "ProtocoledUsers")]
		[HttpPost("createsession")]
		public async Task<IActionResult> CreateSessionAsync([FromBody]CreateMessangerSessionRequest model)
		{
			var user = await GetUserAsync();
			var resutl = await sessionService.MakeSessionAsync(model, user.Id, TokenId);

			return Ok(resutl);
		}


		[Authorize(Roles = "ProtocoledUsers")]
		[HttpGet("setsession/{sessionId}")]
		public async Task<IActionResult> CacheSessionAsync(string sessionId)
		{
			var user = await GetUserAsync();
			await sessionService.CacheSessionKeyAsync(user.Id, sessionId, TokenId);
			
			return Ok();
		} 
	}
}
