using Api.Dal;
using Api.DAL;
using Api.DAL.Base;
using Api.Models.Options;
using Api.Models.ViewModel.Messanger;
using Api.Providers;
using Api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Provider
{
	public interface IMessageProvider : IBaseProvider<Message>
	{
		Task<List<MessageViewModel>> GetMessagesAsync(User user, CancellationToken cts);
	}
	public class MessageProvider : BaseProvider<Message>, IMessageProvider
	{
		private readonly IOptionsMonitor<MessangerOptions> messangerOptions;

		public MessageProvider(ApplicationContext context,
			IOptionsMonitor<MessangerOptions> messangerOptions) : base(context) 
		{
			this.messangerOptions = messangerOptions;
		}

		public async override Task<Message> CreateOrUpdateAsync(Message item)
		{
			await AddAsync(item);
			await db.SaveChangesAsync();

			return item;
		}

		public async Task<List<MessageViewModel>> GetMessagesAsync(User user, CancellationToken cts)
        {
			int maxCountDialogInPage = 30;
			int countDialogsInPage = 10 > maxCountDialogInPage ? maxCountDialogInPage : 10;

			var userDialogs = await db.Dialogs.Where(x => x.User1Id == user.Id || x.User2Id == user.Id)
				.ToListAsync();

			if (userDialogs.IsListNull())
				return null;

			cts.ThrowIfCancellationRequested();

			List<MessageViewModel> result = new List<MessageViewModel>();

			foreach (var userDialog in userDialogs.Select(x => x.Id))
            {
				cts.ThrowIfCancellationRequested();

				var dialog = await db.Dialogs.Where(x => x.Id == userDialog)
					.Select(x => new { x.User1Id, x.User2Id })
					.FirstOrDefaultAsync();

				string userId = dialog.User1Id == user.Id ? dialog.User2Id : dialog.User1Id;
				var currentUser = await db.Users.Where(x => x.Id == userId)
					.Select(x => x.UserName)
					.FirstOrDefaultAsync();

				var lastMessage = await db.Messages.Where(x => x.Created == db.Messages.Where(x => x.DialogId == userDialog).Max(d => d.Created))
					.Select(x => new MessageViewModel()
					{
						Created = x.Created,
						CryptedText = x.MessageText,
						DialogName = currentUser
					}).FirstOrDefaultAsync();

				if (lastMessage == null)
					continue;

				result.Add(lastMessage);
            }
			return result;
		}
	}
}
