using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Providers
{
	public static class QuarableExtensions
	{
		public static async Task<List<T>> Pagination<T>(this IQueryable<T> quearable, 
			int currentPage,
			int countItemInPage) where T : class
		{
			var result = new List<T>();
		
			if (currentPage == 0 || currentPage == 1)
				result = await quearable.AsNoTracking()
					.Take(countItemInPage)
					.ToListAsync();
			else
				result = await quearable.AsNoTracking()
					.Skip((currentPage - 1) * countItemInPage)
					.Take(countItemInPage)
					.ToListAsync();
			return result;
		}
	}
}

