using Api.Dal;
using Api.Dal.Accounting;
using Api.DAL.Base;
using Api.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider
{
	public interface IConnectionProvider : IBaseProvider<Connection>
	{
		Task RemoveAsync(Connection connection);
		Task<int> CountConnectionsAsync(string userId);
	}
	
	public class ConnectionProvider : BaseProvider<Connection>, IConnectionProvider
	{
		
		public ConnectionProvider(ApplicationContext db) : base(db)
		{
		}

		public async Task<int> CountConnectionsAsync(string userId)
		{
			var result = await db.Connections.CountAsync(x => x.UserId == userId);
			return result;
		}

		public async override Task<Connection> CreateOrUpdateAsync(Connection item)
		{
			await AddAsync(item);
			await db.SaveChangesAsync();

			return item;
		}


		public async override Task DeleteAsync(long id)
		{
			await DeleteAsync(id);
			await db.SaveChangesAsync();
		}

		public async Task RemoveAsync(Connection connection)
		{
			db.Connections.Remove(connection);
			await db.SaveChangesAsync();
		}
	}
}
