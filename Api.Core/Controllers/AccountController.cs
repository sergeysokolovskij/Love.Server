using Api.Models.Options;
using Api.Models.RequestModels.AccountController;
using Api.Providers;
using Api.Services.Auth;
using Api.Services.Authentication;
using Api.Services.Exceptions;
using Api.Utils;
using Apis.Utils;
using Apis.Utils.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using ShopPlatform.Controllers;
using ShopPlatform.Models.RequestModels;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : BaseController
	{
		private readonly IAuthService authService;
		private readonly IOptions<UrlPathOptions> urlPathOptions;

		private readonly ILogger<AccountController> logger;
		public AccountController(IServiceProvider serviceProvider,
			IAuthService authService,
			IUserProvider userProvider,
			IOptions<UrlPathOptions> urlPathOptions,
			ILoggerFactory loggerFactory) : base(serviceProvider, userProvider)
		{
			this.authService = authService;
			this.urlPathOptions = urlPathOptions;
			this.logger = loggerFactory.CreateLogger<AccountController>();
		}

	
		//стандарт для веба
		[HttpPost("signin")]
		public async Task<IActionResult> SignInAsync([FromForm]LoginModel model)
		{
			try
			{
				string longSessionValue = "";

				if (!string.IsNullOrEmpty(model.RefreshToken))
					longSessionValue = model.RefreshToken;
				else if (HttpContext.Request.Cookies.TryGetValue(AuthConstants.LongSession, out string _longSessionValue)) // если заход с браузера
					longSessionValue = _longSessionValue;
			
				var userAgent = HttpUtilities.GetDataFromHeaders(HttpContext, HttpConstants.UserAgentHeaderName);
				var fingerPrint = authService.GetFingerPrint(userAgent, longSessionValue);
				var authResult = await authService.AuthorizeByPasswordAsync(model.UserName, model.Password,longSessionValue, userAgent);

				authService.MakeLongSessionCookies(authResult.RefreshToken);

				return Json(authResult);
			}
			catch (ApiError infoException)
			{
				return Unauthorized(infoException.ex.Description);
			}
		}

		//стандарт для веба
		[HttpPost("signup")]
		public async Task<IActionResult> SignUpAsync([FromForm]RegisterModel model)
		{
			try
			{
				var longSession = CryptoRandomizer.GetRandomString(32);
				var fingerPrint = authService.GetFingerPrint(HttpUtilities.GetDataFromHeaders(HttpContext, HttpConstants.UserAgentHeaderName), longSession);

				var registerResult = await authService.RegisterAsync(model.UserName, model.Email, model.Password, fingerPrint, longSession);

				authService.MakeLongSessionCookies(registerResult.RefreshToken);

				return Json(registerResult);
			}
			catch(ApiError infoExcetpion)
			{
				return Unauthorized(infoExcetpion.ex.Description);
			}
		}	

		[HttpPost("longsessionupdate")]
		public async Task<IActionResult> LongSessionUpdateAsync([FromBody] RefreshTokenUpdateModel model)
		{
			try
			{
				string refreshToken = "";
				if (!string.IsNullOrEmpty(model.RefreshToken))
					refreshToken = model.RefreshToken;
				else if (!HttpContext.Request.Cookies.TryGetValue(AuthConstants.LongSession, out string _refreshToken))
					refreshToken = _refreshToken;
				else
					return Redirect(Path.Combine(urlPathOptions.Value.Site, "Account/login"));

				var fingerPrint = authService.GetFingerPrint(HttpUtilities.GetDataFromHeaders(HttpContext, HttpConstants.UserAgentHeaderName), refreshToken);
				var userAgent = HttpUtilities.GetDataFromHeaders(HttpContext, HttpConstants.UserAgentHeaderName);
				var result = await authService.UpdateLongSessionAsync(refreshToken, fingerPrint, userAgent);

				authService.MakeLongSessionCookies(result.RefreshToken);

				return Json(result);
			}
			catch(ApiError infoException)
			{
				return JsonExceptionMessage(infoException.ex.Description);
			}
		}

		[HttpGet("confirmregister/token/{confirmToken}")]
		public async Task<IActionResult> ConfirmRegistrationAsync(string confirmToken)
		{
			try
			{
				var result = await authService.ConfirmRegisterAsync(confirmToken);
				if (result)
					return Ok(); 
			}
			catch
			{
			}
			return BadRequest();
		}

		[HttpGet("logout")]
		public IActionResult LogoutAsync()
		{
			authService.MakeLogoutCookies();
			return Ok(); 
		}
		
		[Authorize]
		[HttpGet("getauthuserinfo")]
		public async Task<IActionResult> GetAuthUserInfo()
		{
			if (!User.Identity.IsAuthenticated)
				return Unauthorized();

			var currentUser = await userProvider.GetModelBySearchPredicate(x => x.UserName == UserName);
			string currentSessionId = TokenId;

			return Json(new
			{
				Login = UserName,
				Roles = RoleNames,
				IsPhoneConfirmed = currentUser.PhoneNumberConfirmed,
				IsEmailConfirmed = currentUser.EmailConfirmed
			});	
		}


		[HttpGet]
		[Authorize]
		public async Task<IActionResult> MakePhoneConfirmTokenAsync()
		{
			return Ok();
		}
	}
}
