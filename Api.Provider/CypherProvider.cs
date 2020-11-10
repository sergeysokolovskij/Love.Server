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
		Dictionary<string, byte[]> GetAllCyphers();
	}
	public class CypherProvider : BaseProvider<Cypher>, ICypherProvider
	{
		public CypherProvider(ApplicationContext dbContext) : base(dbContext)
		{
		}

		public async override Task<Cypher> CreateOrUpdateAsync(Cypher item)
		{
			var savedItem = await db.Cyphers.FirstOrDefaultAsync(x => x.Name == item.Name);

			if (savedItem == null)
			{
				savedItem = new Cypher()
				{
					Name = item.Name,
					Secret = item.Secret
				};
				await db.Cyphers.AddAsync(savedItem);
			}
			else
			{
				savedItem.Secret = item.Secret;
				db.Entry(savedItem).State = EntityState.Modified;
			}
			await db.SaveChangesAsync();
			return savedItem;
		}
		

		public Dictionary<string, byte[]> GetAllCyphers()
		{
			return db.Cyphers.AsNoTracking()
				.ToDictionary(x => x.Name, x=>x.Secret);
		}
	}
}
