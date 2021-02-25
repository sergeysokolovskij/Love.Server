using Api.Dal.Auth;
using Api.DAL;
using Api.DAL.Base;
using Api.Providers;
using Api.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider
{
	public interface IStrongKeyProvider : IBaseProvider<StrongKey>
	{
		Task<long> GetStrongKeyIdAsync(string userId);
		Task<Cypher> GetStrongKeyAsync(string userId);
	}
	public class StrongKeyProvider : BaseProvider<StrongKey>, IStrongKeyProvider
	{
		public StrongKeyProvider(ApplicationContext db) : base(db)
		{
		}

		public async override Task<StrongKey> CreateOrUpdateAsync(StrongKey item)
		{
			await AddAsync(item);
			await db.SaveChangesAsync();

			return item;
		}

		public async Task<long> GetStrongKeyIdAsync(string userId)
		{
			var strongKey = await db.StrongKeys.FirstOrDefaultAsync(x => x.UserId == userId);
			if (strongKey == null)
				throw new ApiError(new ServerException("Strong key does not exist"));

			var cypher = await db.Cyphers.Where(x => x.Id == strongKey.CypherId).Select(x => x.Id).FirstOrDefaultAsync();
			return cypher;
		}

		public async Task<Cypher> GetStrongKeyAsync(string userId)
		{
			long cypherId = await GetStrongKeyIdAsync(userId);
			return await db.Cyphers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cypherId);
		}
	}
}
