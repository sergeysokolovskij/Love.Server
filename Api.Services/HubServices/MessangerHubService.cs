using Api.Models.Messanger;
using Api.Provider.Messanger;
using Api.Providers;
using Api.Services.Brocker;
using Api.Services.Cache;
using Api.Services.Exceptions;
using Api.Services.Messanger;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.HubServices
{
	public interface IMessangerHubService 
	{
		Task OnConnectedAsync(string userName, string userIdentefier, string tokenId);
		Task OnDisconnectedAsync(string userName, string userIdentefier);
		Task<Dictionary<string, string>> GetMessageToSendAsync(string message);
	}
	public class MessangerHubService : IMessangerHubService
	{
		private readonly IConnectionCacheService connectionCacheService;
		private readonly ISessionCacheService sessionCacheService;
		private readonly IMessangerService messangerService;

		private readonly IUserProvider userProvider;
		private readonly ISessionProvider sessionProvider;

		private ILogger<MessangerHubService> logger;
		private readonly IBrockerService brockerService;

		public MessangerHubService(IConnectionCacheService connectionCacheService,
			ISessionCacheService sessionCacheService,
			IMessangerService messangerService,
			IBrockerService brockerService,
			IUserProvider userProvider,
			ISessionProvider sessionProvider,
			ILoggerFactory loggerFactory)
		{
			this.messangerService = messangerService;
			this.connectionCacheService = connectionCacheService;
			this.sessionCacheService = sessionCacheService;

			this.userProvider = userProvider;
			this.sessionProvider = sessionProvider;

			this.brockerService = brockerService;

			this.logger = loggerFactory.CreateLogger<MessangerHubService>();
		}

		public async Task OnConnectedAsync(string userName, string userIdentefier, string tokenId)
		{
			var user = await userProvider.GetModelBySearchPredicate(x => x.UserName == userName);
			var sessionFromCache = await sessionCacheService.GetSessionsFromCacheAsync(user.Id, tokenId);

			await connectionCacheService.AddUserConnectionAsync(user.Id, userIdentefier, sessionFromCache.SessionId);

			if (string.IsNullOrEmpty(sessionFromCache.ClientPublicKey) || string.IsNullOrEmpty(sessionFromCache.ServerPrivateKey))
				throw new ApiError(new ServerException("API ERROR!!! INCORRECT GET SESSION KEYS"));

			logger.LogInformation($"{userIdentefier} was connected");
		}

		public async Task OnDisconnectedAsync(string userName, string userIdentefier)
		{
			var user = await userProvider.GetModelBySearchPredicate(x => x.UserName == userName);
			await connectionCacheService.RemoveUserConnectionAsync(user.Id, userIdentefier);

			logger.LogInformation($"{userIdentefier} was disconected");
		}

		public async Task<Dictionary<string, string>> GetMessageToSendAsync(string message)
		{
			MessageDto model = JsonConvert.DeserializeObject<MessageDto>(message);

			var messagesToSend = await messangerService.GetMessagesToSendAsync(message); // SessionId - key, message - value
			var connections = await connectionCacheService.GetUserConnectionsAsync(model.Message.ReceiverId); // ConnectionId - key, SessionId - value

			Dictionary<string, string> result = new Dictionary<string, string>(); //ключ - id-conneciton, значение - закриптованное сообщение
			
			foreach (var messageToSend in messagesToSend)
			{
				string currentConnection = connections?.Values?.FirstOrDefault(x => x == messageToSend.Key) ?? "";

				if (string.IsNullOrEmpty(currentConnection)) // сейчас юзер с такой сессией находится в оффлайн либо в фоне
				{
					//brockerService.PublishMessage(messageToSend.Key, messageToSend.Value);
					continue;
				}

				result.Add(connections.FirstOrDefault(x => x.Value == currentConnection).Key, messageToSend.Value);
			}
			return result;
		}
	}
}
