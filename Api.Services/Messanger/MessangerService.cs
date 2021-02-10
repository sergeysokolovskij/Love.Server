using Api.Dal;
using Api.Dal.Messanger;
using Api.DAL;
using Api.Models.Messanger;
using Api.Models.Options;
using Api.Provider;
using Api.Providers;
using Api.Services.Cache;
using Api.Services.Crypt;
using Api.Services.Exceptions;
using Api.Services.Logs;
using Api.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Api.Services.Messanger
{
	public interface IMessangerService
	{
		Task<IDictionary<string, string>> GetMessagesToSendAsync(string message);
		IAsyncEnumerable<List<string>> GetUserToNotifyOnlineAsync(string userId);
	}
	public class MessangerService : IMessangerService
	{
		private readonly IUserProvider userProvider;

		private readonly IMessangerCryptor messangerCryptor;
		private readonly ICypherProvider cypherProvider;
		private readonly IStrongKeyProvider strongKeyProvider;

		private readonly IDialogProvider dialogProvider;
		private readonly IMessageProvider messageProvider;

		private readonly IConnectionCacheService connectionCacheService;

		private readonly ILogger<MessangerService> logger;
		

		public MessangerService(
			IUserProvider userProvider,
			ICypherProvider cypherProvider,
			IMessageProvider messageProvider,
			IDialogProvider dialogProvider,
			IStrongKeyProvider strongKeyProvider,
			IMessangerCryptor messangerCryptor,
			IConnectionCacheService connectionCacheService,
			ILoggerFactory loggerFactory
			)
		{
			this.userProvider = userProvider;
			this.cypherProvider = cypherProvider;
			this.messangerCryptor = messangerCryptor;
			this.messageProvider = messageProvider;
			this.logger = loggerFactory.CreateLogger<MessangerService>();
			this.strongKeyProvider = strongKeyProvider;
			this.dialogProvider = dialogProvider;
			this.connectionCacheService = connectionCacheService;
		}

		private async Task<Message> SaveMessageAsync(DecryptedMessageDto message)
		{
			var dialog = await dialogProvider.GetDialogAsync(message.SenderId, message.ReceiverId);
			if (dialog == null)
			{
				dialog = new Dialog()
				{
					User1Id = message.SenderId,
					User2Id = message.ReceiverId
				};
				await dialogProvider.CreateOrUpdateAsync(dialog);
			}

			var cypher = await cypherProvider.CreateOrUpdateAsync(new Cypher()
			{
				Secret = message.Aes.FromUrlSafeBase64()
			});

			var model = new Message()
			{
				MessageId = message.MessageId,
				Created = DateTime.Now,
				MessageText = message.CryptedText,
				CypherId = cypher.Id,
				SenderId = message.SenderId,
				ReceiverId = message.ReceiverId,
				DialogId = dialog.Id,
				IsReaded = false
			};
			
			await messageProvider.CreateOrUpdateAsync(model);
			return model;
		}		

		public async Task<IDictionary<string, string>> GetMessagesToSendAsync(string message)
		{
			var model = JsonConvert.DeserializeObject<MessageDto>(message);
			var decryptedMessage = await messangerCryptor.DecryptMessageAsync(model);

			var savedMessage = await SaveMessageAsync(decryptedMessage);

			var result = await messangerCryptor.CryptMessageAsync(new MessageBuildDto()
			{
				MessageId = savedMessage.Id.ToString(),
				Aes = decryptedMessage.Aes,
				SenderId = decryptedMessage.SenderId,
				ReceiverId = decryptedMessage.ReceiverId,
				SessionId = model.Message.SessionId,
				Text = model.Message.CryptedText
			});
			return result;
		}

		public async IAsyncEnumerable<List<string>> GetUserToNotifyOnlineAsync(string userId)
		{         
			var dialogs = await dialogProvider.GetModelsBySearchPredicate(x => x.User1Id == userId || x.User2Id == userId);
			if (dialogs == null || dialogs.Count == 0)
				yield break;

			foreach (var dialog in dialogs)
			{
				string currentId = dialog.User1Id == userId ? dialog.User2Id : dialog.User1Id;

				var userConnections = await connectionCacheService.GetUserConnectionsAsync(currentId);
				if (userConnections != null && userConnections.Count > 0)
				{
					List<string> result = new List<string>();
					foreach (var userConnection in userConnections)
						result.Add(userConnection.Key);

					yield return result;
				}
			}
		}

		public async Task GetSyncDataAsync(string userId)
        {
			
        }
	}
}
