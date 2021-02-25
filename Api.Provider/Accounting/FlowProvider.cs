using Api.Dal.Accounting;
using Api.DAL;
using Api.DAL.Base;
using Api.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider.Accounting
{
	public interface IFlowProvider
	{
		Task CreateFlowAsync(long fromId,
			User fromUser,
			string accountingPlan,
			long? toId,
			User toUser = null,
			string subConto1 = null,
			string subConto2 = null,
			string comment = null,
			string followInformation = null);
	}
	public class FlowProvider : IFlowProvider 
	{
		private readonly ApplicationContext db;

		public FlowProvider(ApplicationContext db)
		{
			this.db = db;
		}


		public async Task CreateFlowAsync(long fromId,
			User fromUser,
			string accountingPlan,
			long? toId, 
			User toUser = null,
			string subConto1 = null,
			string subConto2 = null,
			string comment = null,
			string followInformation = null)
		{
			var accPlan = await db.AccountingPlans.FirstOrDefaultAsync(x => x.Name == accountingPlan);
			if (accPlan == null)
			{
				db.AccountingPlans.Add(new AccountingPlan() 
				{
					Name = accountingPlan
				});
				await db.SaveChangesAsync();
			}

			IQueryable<AccountingRecord> fromQuery = db.AccountingRecords.Where(x => x.Id == fromId && x.AccountingPlanId == accPlan.Id);

			if (!string.IsNullOrEmpty(subConto1))
				fromQuery = db.AccountingRecords.Where(x => x.SubConto1 == subConto1);
			if (!string.IsNullOrEmpty(subConto2))
				fromQuery = db.AccountingRecords.Where(x => x.SubConto2 == subConto2);

			var from = await fromQuery.SingleOrDefaultAsync();

			if (from == null)
			{
				db.AccountingRecords.Add(new AccountingRecord()
				{
					AccountingPlanId = accPlan.Id,
					UserId = fromUser.Id,
					SubConto1 = subConto1,
					SubConto2 = subConto2
				});
				await db.SaveChangesAsync();
			}

			Flow result = new Flow();

			var currentUser = await db.Users.FirstOrDefaultAsync(x => x.Id == from.UserId);
			if (currentUser == null)
				throw new ApiError(new ServerException("API ERROR!!!"));

			AccountingRecord to = null;
			if (toId.HasValue)
			{

				IQueryable<AccountingRecord> toQuery = db.AccountingRecords.Where(x => x.Id == toId.Value);
				if (!string.IsNullOrEmpty(subConto1))
					toQuery = db.AccountingRecords.Where(x => x.SubConto1 == subConto1);
				if (!string.IsNullOrEmpty(subConto2))
					toQuery = db.AccountingRecords.Where(x => x.SubConto2 == subConto2);

				to = await toQuery.SingleOrDefaultAsync();

				if (to == null && toUser == null)
					throw new ApiError(new ServerException("Incorrect accounting record"));

				to = new AccountingRecord()
				{
					UserId = toUser.Id,
					AccountingPlanId = accPlan.Id,
					SubConto1 = subConto1,
					SubConto2 = subConto2
				};
				db.AccountingRecords.Add(to);
				await db.SaveChangesAsync();

				result.ToId = to.Id;
			}

			AccountingComment accComment = null;

			if (!string.IsNullOrEmpty(comment))
			{
				accComment = await db.AccountingComments.FirstOrDefaultAsync(x => x.Comment == comment);
				if (accComment == null)
				{
					db.AccountingComments.Add(new AccountingComment()
					{
						Comment = comment
					});
					await db.SaveChangesAsync();
				}
				result.AccountingCommentId = accComment.Id;
			}

			if (!string.IsNullOrEmpty(followInformation))
				result.FollowInformation = followInformation;

			db.Flows.Add(result);
			await db.SaveChangesAsync();
		}


		public async Task<List<Flow>> GetStatisticAsync()
		{
			var result = new List<Flow>();
			return result;
		}
	}
}
