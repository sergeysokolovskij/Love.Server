using Api.DAL.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Providers
{
	public interface IBaseProvider<T>
	{
		Task<T> CreateOrUpdateAsync(T item);
		Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate);
		Task DeleteAsync(long id);
	}
	public class BaseProvider<T> : IBaseProvider<T> where T :class
	{
		private readonly ApplicationContext dbContext;
		private readonly DbSet<T> dbSet;

		public BaseProvider(ApplicationContext dbContext)
		{
			this.dbContext = dbContext;
			dbSet = db.Set<T>();
		}

		protected ApplicationContext db
		{
			get
			{
				return dbContext;
			}
		}

		public virtual Task<T> CreateOrUpdateAsync(T item)
		{
			throw new NotImplementedException("Method has to be overrride");
		}

		public virtual Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate)
		{
			return
				dbSet.Where(predicate).FirstOrDefaultAsync();
		}

		public virtual async Task DeleteAsync(long id)
		{
			var entity = await dbSet.FindAsync(id);
			dbSet.Remove(entity);
			await db.SaveChangesAsync();
		}
	}
}
