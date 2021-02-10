using Api.Dal.Auth;
using Api.DAL.Base;
using Api.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider.Messanger
{
	public interface ISessionProvider : IBaseProvider<Session>
	{

	}
	public class SessionProvider : BaseProvider<Session>, ISessionProvider
	{
		private readonly ILogger<SessionProvider> logger;

		public SessionProvider(ApplicationContext db,
			ILoggerFactory loggerFactory) : base(db)
		{
			logger = loggerFactory.CreateLogger<SessionProvider>();
		}

		public async override Task<Session> CreateOrUpdateAsync(Session item)
		{
			await AddAsync(item);
			await db.SaveChangesAsync();

			return item;
		}
	}
}
