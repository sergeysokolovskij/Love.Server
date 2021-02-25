using Api.Models.Messanger;
using Api.Provider;
using Api.Providers;
using Api.Services.Cache;
using Api.Services.Crypt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Api.Utils;
using System.Collections.Concurrent;
using Api.Provider.Messanger;
using Api.Dal.Auth;
using Api.Services.Exceptions;

namespace Api.Services.Messanger
{
	public interface IMessangerCryptor
	{
		Task<IDictionary<string, string>> CryptMessageAsync(MessageBuildDto message);
		Task<DecryptedMessageDto> DecryptMessageAsync(MessageDto model);
	}

	public class MessangerCryptor : IMessangerCryptor
	{
		private readonly IAesCipher aes;
		private readonly IRsaCypher rsa;

		private readonly ICypherProvider cypherProvider;
		private readonly IStrongKeyProvider strongKeyProvider;

		private readonly ISessionCacheService sessionCacheService;
		private readonly ISessionProvider sessionProvider;

		private readonly IServiceProvider serviceProvider;

		public MessangerCryptor(
			IServiceProvider serviceProvider,
			IAesCipher aes,
			IRsaCypher rsa,
			ISessionCacheService sessionCacheService,
			ICypherProvider cypherProvider,
			ISessionProvider sessionProvider,
			IStrongKeyProvider strongKeyProvider)
		{
			this.serviceProvider = serviceProvider;

			this.aes = aes;
			this.rsa = rsa;

			this.sessionCacheService = sessionCacheService;
			this.cypherProvider = cypherProvider;
			this.strongKeyProvider = strongKeyProvider;
			this.sessionProvider = sessionProvider;
		}


		//TODO::: Эти данные тоже неплохо было бы из кеш-сервиса брать
		private async Task<Session> GetCurrentSessionAsync(string userId, string session)
		{
			var sessions = await sessionProvider.GetModelsBySearchPredicate(x => x.UserId == userId);
			foreach (var currentSession in sessions)
				if (currentSession.SessionId == session)
					return currentSession;
			return null;
		}

		public async Task<IDictionary<string, string>> CryptMessageAsync(MessageBuildDto message) // крипт сообщений. используется для отправки сообщения от сервера
		{
			var strongKey = await strongKeyProvider.GetStrongKeyAsync(message.ReceiverId);
			string cryptedAesKey = aes.Crypt(strongKey.Secret.ToUrlSafeBase64(), message.Aes);

			var result = new ConcurrentDictionary<string, string>();
			var tasks = new List<Task>();

			var sessions = await sessionProvider.GetModelsBySearchPredicate(x => x.UserId == message.ReceiverId);

			foreach (var session in sessions)
			{
				tasks.Add(Task.Run(() =>
				{
					var signMessage = new SignMessageDto()
					{
						MessageId = message.MessageId,
						CryptedText = message.Text,
						CryptedAes = cryptedAesKey,
						ReceiverId = message.ReceiverId,
						SenderId = message.SenderId,
						SessionId = message.SessionId,
						Created = DateTime.Now
					};

					string sign = rsa.SignData(session.ServerPrivateKey, signMessage.ObjectToBytes());

					var item = new MessageDto()
					{
						Message = signMessage,
						Sign = sign
					};

					result.TryAdd(session.SessionId, JsonConvert.SerializeObject(item));
				}));
			}
			await Task.WhenAll(tasks);
			return result;
		}

		public async Task<DecryptedMessageDto> DecryptMessageAsync(MessageDto model)
		{
			var secretKey = await strongKeyProvider.GetStrongKeyAsync(model.Message.SenderId);
		
			string decryptedAesKey = aes.Decrypt(secretKey.Secret.ToUrlSafeBase64(), model.Message.CryptedAes);
			var session = await GetCurrentSessionAsync(model.Message.SenderId, model.Message.SessionId);

			if (rsa.VerifySignature(session.ClientPublicKey, model.Message.ObjectToBytes(), model.Sign.FromUrlSafeBase64()))
				throw new ApiError(new ServerException("Incorrect signature"));

			return new DecryptedMessageDto()
			{
				MessageId = model.Message.MessageId,
				CryptedText = model.Message.CryptedText,
				Aes = decryptedAesKey,
				ReceiverId = model.Message.ReceiverId,
				SenderId = model.Message.SenderId
			};
		}
	}
}
