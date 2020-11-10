using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Api.Services.Auth;
using Api.Services.Crypt;
using Api.Services.Email;
using Api.Services.Email.Templates;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.DAL;
using Shared;
using Api.Services.Exceptions;
using Api.Providers;
using Api.Models.Options;

namespace Api.Services.Authentication
{
	public interface IAuthService
	{
		Task<AuthResult> AuthorizeByPasswordAsync(
			string userName,
			string password,
			string fingerPrint,
			string newLongSessionValue,
			string userAgent
		);
		Task<AuthResult> RegisterAsync(
			string userName,
			string email,
			string password,
			string fingerPrint,
			string longSession
		);
		Task<AuthResult> UpdateLongSessionAsync(
			string longSession, 
			string fingerPrint,
			string userAgent
		);

		Task<bool> ConfirmRegisterAsync(
			string token
		);
		string GetFingerPrint(
			string userAgent, 
			string longSessionValue
		);
		void MakeLogoutCookies();
		void MakeLongSessionCookies(
			string value
		);
	}

	/*TODO:::Все токены хранить в httponly cookies*/

	public class AuthService : IAuthService
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly RoleManager<IdentityRole> roleManager;

		private readonly IUserProvider userProvider;

		private readonly IJwtService jwtService;
		private readonly IEmailSenderService emailSenderService;

		private readonly IOptions<UrlPathOptions> urlPathOptions;
		private readonly IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions;

		private readonly HttpContext context;

		public AuthService(UserManager<User> userManager, 
			SignInManager<User> signInManager,
			RoleManager<IdentityRole> roleManager,
			IUserProvider userProvider,
			IJwtService jwtService,
			IOptions<UrlPathOptions> urlPathOptions,
			IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions,
			IEmailSenderService emailSenderService,
			IHttpContextAccessor httpContextAccessor
			)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
			this.userProvider = userProvider;
			this.jwtService = jwtService;
			this.urlPathOptions = urlPathOptions;
			this.tokenLifeTimeOptions = tokenLifeTimeOptions;
			this.emailSenderService = emailSenderService;
			this.context = httpContextAccessor.HttpContext;
		}

		public async Task<AuthResult> RegisterAsync(
			string userName, 
			string email,
			string password, 
			string fingerPrint,
			string longSession)
		{
			var isSignIn = await signInManager.PasswordSignInAsync(userName, password, false, false);

			if (isSignIn.Succeeded)
				throw new ApiError(new ServerException("Пользователь с таким логином уже зарегистрирован"));

			var user = new User()
			{
				UserName = userName,
				Email = email
			};

			bool isNeedConfirmEmail = emailSenderService.IsNeedConfirm();

			if (isNeedConfirmEmail)
			{
				var isEmailUnique = await userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
				if (isEmailUnique != null)
					throw new ApiError(new ServerException("Пользователь с таким email уже зарегистрирован"));
			}

			var resultCreate = await userManager.CreateAsync(user, password);

			IdentityRole role;

			if (await userManager.Users.CountAsync() == 1)
				role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
			else
				role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "User");

			await userManager.AddToRoleAsync(user, role.Name);

			var registeredUser = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);

			await userProvider.CreateLongSessionAsync(new LongSession()
			{
				UserId = registeredUser.Id,
				FingerPrint = fingerPrint,
				Value = longSession
			});

			if (isNeedConfirmEmail)
			{
				string emailToken = jwtService.GenereteEmailToken(registeredUser);
				await emailSenderService.SendEmailAsync(new AuthMailInfo(emailToken), registeredUser.Email, MailTempleteNames.AuthMailTemplateName);
			}

			return new AuthResult()
			{
				UserName = userName,
				Role = role.Name,
				AccessToken = jwtService.GenereteJwtToken(userName, registeredUser, role.Name),
				RefreshToken = longSession
			};
		}

		public async Task<AuthResult> AuthorizeByPasswordAsync(
			string userName,
			string password,
			string fingerPrint,
			string newLongSessionValue,
			string userAgent)
		{
			var result = await signInManager.PasswordSignInAsync(userName, password, false, false);

			if (!result.Succeeded)
				throw new ApiError(new ServerException("Некорректный логин и/или пароль"));

			var appUser = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);

			if (emailSenderService.IsNeedConfirm())
				if (!appUser.EmailConfirmed)
					throw new ApiError(new ServerException("Почта не была подтверждена"));

			string role = (await userManager.GetRolesAsync(appUser))[0];

			if (!context.Request.Cookies.TryGetValue(AuthConstants.LongSession, out string longSessionValue))
				await userProvider.CreateLongSessionAsync(new LongSession()
				{
					UserId = appUser.Id,
					FingerPrint = fingerPrint,
					Value = newLongSessionValue
				});
			else
			{
				var newFingerPrint = GetFingerPrint(userAgent, newLongSessionValue);
				await userProvider.UpdateLongSessionAsync(appUser.Id, longSessionValue, fingerPrint, newFingerPrint, newLongSessionValue);
			}
			return (new AuthResult()
			{
				UserName = userName,
				Role = role,
				AccessToken = jwtService.GenereteJwtToken(userName, appUser, role),
				RefreshToken = newLongSessionValue
			});
		}

		public async Task<AuthResult> UpdateLongSessionAsync(string longSession, 
			string fingerPrint, 
			string userAgent)
		{
			var session = await userProvider.GetLongSessionAsync(longSession);

			if (session == null)
				throw new ApiError(new ServerException("Несуществующий идентификатор longsession"));

			var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == session.UserId);

			string newLongSessionValue = jwtService.GenerateRefreshToken();
			string newFingerPrint = GetFingerPrint(userAgent, newLongSessionValue);

			bool isOkey = await userProvider.UpdateLongSessionAsync(user.Id, longSession, fingerPrint, newFingerPrint, newLongSessionValue);

			if (!isOkey)
				throw new ApiError(new ServerException("Несуществующий идентификатор longsession"));

			string role = (await userManager.GetRolesAsync(user))[0];

			return new AuthResult()
			{
				UserName = user.UserName,
				Role = role,
				AccessToken = jwtService.GenereteJwtToken(user.UserName, user, role),
				RefreshToken = newLongSessionValue
			};
		}

		public async Task<bool> ConfirmRegisterAsync(string token)
		{
			string userId;
			try
			{
				var tokenDecrypt = jwtService.DecryptEmailToken(token);
				var jweToken = new JwtSecurityToken(new JwtHeader(), JwtPayload.Deserialize(tokenDecrypt));

				userId = jweToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

				if (jweToken.ValidTo < DateTime.UtcNow)
					return false;

				var updateResult = await userProvider.ConfirmUserRegisterAsync(userId);
				if (updateResult)
					return true;
			}
			catch
			{
			}
			return false; 
		}

		public void MakeLogoutCookies()
		{
			if (context.Request.Cookies.ContainsKey(AuthConstants.LongSession))
				context.Response.Cookies.Delete(AuthConstants.LongSession);
		}

		public void MakeLongSessionCookies(string value)
		{
			context.Response.Cookies.Append(AuthConstants.LongSession, value, new CookieOptions()
			{
				Path = "/",
				HttpOnly = true,
				IsEssential = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = DateTime.Now.AddDays(tokenLifeTimeOptions.Value.RefreshTokenLifeTime)
			});
		}

		public string GetFingerPrint(string userAgent, string longSessionValue)
		{
			var print = new
			{
				UserAgent = userAgent,
				LongSession = longSessionValue
			};
			return Sha1.GetHash(JsonConvert.SerializeObject(print));
		}
	}
}
