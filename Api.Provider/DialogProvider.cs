using Api.Dal.Messanger;
using Api.DAL.Base;
using Api.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider
{
	public interface IDialogProvider: IBaseProvider<Dialog>
	{
		Task<bool> IsDialogExistAsync(string user1Id, string user2Id);
		Task<Dialog> GetDialogAsync(string user1Id, string user2Id);
	}

	public class DialogProvider : BaseProvider<Dialog>, IDialogProvider
	{
		public DialogProvider(ApplicationContext db) : base(db)
		{
		}

		public Task<bool> IsDialogExistAsync(string user1Id, string user2Id)
		{
			return db.Dialogs.AnyAsync(x => x.User1Id == user1Id && x.User2Id == user2Id
				|| x.User1Id == user2Id && x.User2Id == user1Id);
		}

		public Task<Dialog> GetDialogAsync(string user1Id, string user2Id)
		{
			return db.Dialogs.FirstOrDefaultAsync(x => x.User1Id == user1Id && x.User2Id == user2Id
				|| x.User1Id == user2Id && x.User2Id == user1Id);
		}

		public async override Task<Dialog> CreateOrUpdateAsync(Dialog item)
		{
			await AddAsync(item);
			await db.SaveChangesAsync();

			return item;
		}
	}
}
