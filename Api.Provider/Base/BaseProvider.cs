using Api.Dal;
using Api.DAL.Base;
using Api.Provider.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Providers
{
	public interface IBaseProvider<T>
	{
		Task<T> CreateOrUpdateAsync(T item);
		Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate);
		Task<List<T>> GetModelsBySearchPredicate(Expression<Func<T, bool>> predicate);
		Task<List<T>> GetModelsBySearchPredicate(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selected);
		Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector);
		Task DeleteAsync(long id);
		Task<List<T>> GetAllAsync();
		Task DeleteAsync(T item);
		Task DeleteAsync(List<T> items);
	}
	public class BaseProvider<T> : BaseContextProvider ,IBaseProvider<T> where T : class, IBaseEntity
	{
		protected readonly DbSet<T> dbSet;

		public BaseProvider(ApplicationContext db) : base(db)
		{
			dbSet = this.db.Set<T>();
		}


		public async virtual Task<T> CreateOrUpdateAsync(T item)
		{
			item.Created = DateTime.Now;

			dbSet.Add(item);
			await db.SaveChangesAsync();

			return item;
		}

		public virtual Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate)
		{
			return
				dbSet.Where(predicate).FirstOrDefaultAsync();
		}

		public virtual Task<List<T>> GetModelsBySearchPredicate(Expression<Func<T, bool>> predicate)
		{
			return
				dbSet.Where(predicate).ToListAsync();
		}

		public Task<T> GetModelBySearchPredicate(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector)
		{
			return dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
		}

		public Task<List<T>> GetModelsBySearchPredicate(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selected) 
		{
			return dbSet.Where(predicate).Select(selected).ToListAsync();
		}

		public virtual async Task DeleteAsync(long id) 
		{
			var entity = await dbSet.FindAsync(id);
			dbSet.Remove(entity);
		}

		public Task UpdateAsync<TEntity>(TEntity item) where TEntity : class, IBaseEntity
		{
			item.Updated = DateTime.Now;
			db.Entry(item).State = EntityState.Modified;

			return Task.CompletedTask;
		}

		public Task AddAsync<TEntity>(TEntity item) where TEntity: class, IBaseEntity
		{
			var set = db.Set<TEntity>();

			item.Created = DateTime.Now;
			set.Add(item);

			return Task.CompletedTask;
		}


		public Task<List<T>> GetAllAsync()
		{
			return dbSet.AsNoTracking().ToListAsync();
		}

		public async Task DeleteAsync(T item)
		{
			dbSet.Remove(item);
			await db.SaveChangesAsync();
		}

		public async Task DeleteAsync(List<T> items)
		{
			dbSet.RemoveRange(items);
			await db.SaveChangesAsync();
		}
	}
}
