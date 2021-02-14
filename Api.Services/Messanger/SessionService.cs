using Api.Dal.Auth;
using Api.DAL;
using Api.DAL.Base;
using Api.Models.RequestModels.MessangerSessionController;
using Api.Models.ResponseModel.MessangerSessionController;
using Api.Provider;
using Api.Provider.Base;
using Api.Provider.Messanger;
using Api.Providers;
using Api.Services.Cache;
using Api.Services.Crypt;
using Api.Services.Exceptions;
using Api.Services.Messanger.Models;
using Api.Services.Processing;
using Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Messanger
{
	public interface ISessionService
	{
		Task<CreateFirstMessangerSessionResponse> MakeFirstSessionAsync(CreateMessangerSessionRequest model, 
			string userId,
			string sessionId
		);
		Task<CreateMessangerSessionResponse> MakeSessionAsync(CreateMessangerSessionRequest model,
			string userId,
			string sessionId);
		Task CacheSessionKeyAsync(string userId, string sessionId, string tokenId);
	}
	public class SessionService : ISessionService
	{
		private readonly ISessionProvider sessionProvider;

		private readonly IRsaCypher rsaCypher;
		private readonly IAesCipher aesCypher;

		private readonly UserManager<User> userManager;
		private readonly RoleManager<IdentityRole> roleManager;

		private readonly ICypherProvider cypherProvider;
		private readonly IStrongKeyProvider strongKeyProvider;

		private readonly ITransactionProvider transactionProvider;

		private readonly ISessionCacheService sessionCacheService;

		private readonly ProcessingProvider processingProvider;


		public SessionService(ISessionProvider sessionProvider,
			ITransactionProvider transactionProvider,
			IRsaCypher rsaCypher,
			UserManager<User> userManager,
			RoleManager<IdentityRole> roleManager,
			IStrongKeyProvider strongKeyProvider,
			ICypherProvider cypherProvider,
			IAesCipher aesCypher,
			ISessionCacheService sessionCacheService,
			ProcessingProvider processingProvider)
		{
			this.transactionProvider = transactionProvider;
			this.sessionProvider = sessionProvider;
			this.rsaCypher = rsaCypher;
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.strongKeyProvider = strongKeyProvider;
			this.cypherProvider = cypherProvider;
			this.aesCypher = aesCypher;
			this.sessionCacheService = sessionCacheService;
			this.processingProvider = processingProvider;
		}

		public async Task<CreateFirstMessangerSessionResponse> MakeFirstSessionAsync(CreateMessangerSessionRequest model,
			string userId,
			string sessionId)
		{
			var savedSessions = await sessionProvider.GetModelBySearchPredicate(x => x.SessionId == sessionId 
				&& x.UserId == userId);

			if (savedSessions != null)
				throw new ApiError(new ServerException("This session is alredy exist"));

			var serverKeys = rsaCypher.GenerateKeys();
			var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

			var serverSession = new Session()
			{
				ClientPublicKey = model.PublicKey,
				ServerPrivateKey = serverKeys.privateKey,
				ServerPublicKey = serverKeys.publicKey,
				SessionId = sessionId,
				UserId = userId
			};

			var savedStrongKey= await strongKeyProvider.GetModelBySearchPredicate(x => x.UserId == userId);
			if (savedStrongKey != null)
			{
				var savedCypher = await cypherProvider.GetModelBySearchPredicate(x => x.Id == savedStrongKey.CypherId);
				await sessionProvider.CreateOrUpdateAsync(serverSession);

				return new CreateFirstMessangerSessionResponse()
				{
					ServerPublicKey = serverKeys.publicKey,
					CryptedAes = rsaCypher.Crypt(model.PublicKey, savedCypher.Secret.ToUrlSafeBase64())
				};
			}
			else 
			{
				await using (var transaction = await transactionProvider.BeginTransactionAsync())
				{
					try
					{
						await sessionProvider.CreateOrUpdateAsync(serverSession);
						var savedRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "ProtocoledUsers");

						await userManager.AddToRoleAsync(user, savedRole.Name);

						byte[] strongKey = CryptoRandomizer.GenerateSecurityKey(16);

						var cypher = await cypherProvider.CreateOrUpdateAsync(new Cypher()
						{
							Secret = strongKey
						});

						var key = await strongKeyProvider.CreateOrUpdateAsync(new StrongKey()
						{
							CypherId = cypher.Id,
							UserId = user.Id
						});

						string strongKeyToCrypt = strongKey.ToUrlSafeBase64();
						string cryptedAesKey = rsaCypher.Crypt(model.PublicKey, strongKeyToCrypt);

						await transaction.CommitAsync();

						return new CreateFirstMessangerSessionResponse()
						{
							ServerPublicKey = serverKeys.publicKey,
							CryptedAes = cryptedAesKey
						};
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new ApiError(new ServerException(ex.Message));
					}
				}
			}
		}

		public async Task<CreateMessangerSessionResponse> MakeSessionAsync(CreateMessangerSessionRequest model,
			string userId,
			string sessionId)
		{
			var strongKey = await strongKeyProvider.GetModelBySearchPredicate(x => x.UserId == userId);
			string decryptedPublicKey = await aesCypher.DecryptString(strongKey.CypherId, model.PublicKey);

			var serverKeys = rsaCypher.GenerateKeys();

			var serverSession = new Session()
			{
				ClientPublicKey = decryptedPublicKey,
				ServerPrivateKey = serverKeys.privateKey,
				ServerPublicKey = serverKeys.publicKey,
				SessionId = sessionId,
				UserId = userId
			};

			await sessionProvider.CreateOrUpdateAsync(serverSession);

			string cryptedPublicKey = await aesCypher.Crypt(strongKey.CypherId, serverKeys.publicKey);
			string cryptedSessionId = await aesCypher.Crypt(strongKey.CypherId, serverSession.SessionId);

			return new CreateMessangerSessionResponse()
			{
				ServerPublicKey = cryptedPublicKey,
				SessionId = cryptedSessionId
			};
		}

		public async Task CacheSessionKeyAsync(string userId, string sessionId, string tokenId)
		{
			var strongKey = await strongKeyProvider.GetStrongKeyIdAsync(userId);
			string decryptedSessionId = await aesCypher.DecryptString(strongKey, sessionId);

			var session = await sessionProvider.GetModelBySearchPredicate(x => x.SessionId == decryptedSessionId && x.UserId == userId);
			if (session == null)
				throw new ApiError(new ServerException("Incorrect session"));

			await sessionCacheService.CacheSessionKeysAsync(tokenId, userId, decryptedSessionId);

			await processingProvider.readMessagesProcessingHandler.HandleAsync(decryptedSessionId);
		}
	}
}
