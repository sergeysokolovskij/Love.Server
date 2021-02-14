using Api.Models.Messanger;
using Api.Provider.Messanger;
using Api.Providers;
using Api.Services.Cache;
using Api.Services.Exceptions;
using Api.Services.HubServices;
using Api.Services.Messanger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Api.Utils;
using CommonConstants = Api.Utils.CommonConstants;
using Newtonsoft.Json;
using System.Threading;

namespace Api.Core.Hubs.Messanger
{
	public class MessangerHub : BaseHub
	{
		private readonly IMessangerHubService messangerHub;
		private readonly IMessangerService messangerService;

		public MessangerHub(
			IServiceProvider serviceProvider,
			IMessangerHubService messangerHub,
			IMessangerService messangerService) : base(serviceProvider)
		{
			this.messangerHub = messangerHub;
			this.messangerService = messangerService;
		}

		[Authorize(Roles = "ProtocoledUsers")]
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			string tokenId = Context.User.Claims.Where(x => x.Type == CommonConstants.UniqueClaimName).Select(x => x.Value).FirstOrDefault();

			await messangerHub.OnConnectedAsync(Context.User.Identity.Name, Context.ConnectionId, tokenId);

			var currentUser = await GetCurrentUserAsync();

			await foreach(var userConnections in messangerService.GetUserToNotifyOnlineAsync(currentUser.Id))
            {
				foreach (var userConnection in userConnections)
					await Clients.Client(userConnection).SendAsync("UserOnlineNotify", currentUser.Id);
            }
		}

		
		[Authorize(Roles = "ProtocoledUsers")]
		public async override Task OnDisconnectedAsync(Exception exception)
		{
			await base.OnDisconnectedAsync(exception);
			await messangerHub.OnDisconnectedAsync(Context.User.Identity.Name, Context.ConnectionId);

			var currentUser = await GetCurrentUserAsync();

			await foreach (var userConnections in messangerService.GetUserToNotifyOnlineAsync(currentUser.Id))
			{
				foreach (var userConnection in userConnections)
					await Clients.Client(userConnection).SendAsync("UserOfflineNotify", currentUser.Id);
			}
		}

		[Authorize(Roles = "ProtocoledUsers")]
		[HubMethodName("SendMessage")]
		public async Task OnSendMessageAsync(string message)
		{
			var messagesToSend = await messangerHub.GetMessageToSendAsync(message);
			if (messagesToSend == null)
				return;

			foreach (var messageToSend in messagesToSend)
				await Clients.Client(messageToSend.Key).SendAsync("ReceiveMesssage", messageToSend.Value);
		}

		[Authorize(Roles = "ProtocoledUsers")]
		[HubMethodName("getmessages")]
		public async Task GetDialogNewMessagesAsync()
		{
		}
	}
}
