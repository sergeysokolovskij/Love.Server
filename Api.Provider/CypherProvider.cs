using Api.DAL;
using Api.DAL.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Providers
{
	public interface ICypherProvider : IBaseProvider<Cypher>
	{
	}
	public class CypherProvider : BaseProvider<Cypher>, ICypherProvider
	{
		public CypherProvider(ApplicationContext dbContext) : base(dbContext)
		{
		}

		public async override Task<Cypher> CreateOrUpdateAsync(Cypher item)
		{
			var savedItem = await db.Cyphers.FirstOrDefaultAsync(x => x.Id == item.Id);

			if (savedItem == null)
			{
				savedItem = new Cypher()
				{
					Secret = item.Secret
				};

				await AddAsync(savedItem);
			}
			else
			{
				savedItem.Secret = item.Secret;
				await UpdateAsync(savedItem);
			}
			await db.SaveChangesAsync();
			return savedItem;
		}
		
	}
}
