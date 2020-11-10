using Api.DAL;
using Api.DAL.Base;
using Api.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Providers
{
	public interface IUserProvider : IBaseProvider<User>
 	{
		Task<bool> CreateLongSessionAsync(LongSession longSession);
		Task<bool> IsLongSessionExistAsync(string userId, string longSession, string fingerPrint);
		Task<bool> UpdateLongSessionAsync(string userId,
			string refreshToken,
			string fingerPrint,
			string newFingerPrint,
			string newLongSessionValue);
		Task<LongSession> GetLongSessionAsync(string value);
		Task<bool> ConfirmUserRegisterAsync(string userId);
	}

	public class UserProvider : BaseProvider<User>, IUserProvider
	{
		private readonly IOptions<TokenLifeTimeOptions> tokenOptions;
		public UserProvider(ApplicationContext dbContext,
			IOptions<TokenLifeTimeOptions> tokenOptions) : base(dbContext)
		{
			this.tokenOptions = tokenOptions;
		}


		public async Task<bool> CreateLongSessionAsync(LongSession longSession)
		{
			var savedItem = await db.LongSessions.FirstOrDefaultAsync(x => x.UserId == longSession.UserId &&
									  x.FingerPrint == longSession.FingerPrint);
			if (savedItem != null)
				return false;

			savedItem = new LongSession();

			savedItem.UserId = longSession.UserId;
			savedItem.FingerPrint = longSession.FingerPrint;
			savedItem.Value = longSession.Value;

			savedItem.CreatedAt = DateTime.Now;
			savedItem.ExpiresIn = DateTime.Now.AddDays(tokenOptions.Value.RefreshTokenLifeTime);

			await db.LongSessions.AddAsync(savedItem);

			return await db.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateLongSessionAsync(string userId, 
			string longSession, 
			string fingerPrint,
			string newFingerPrint,
			string newLongSessionValue)
		{
			var savedItem = await db.LongSessions.FirstOrDefaultAsync(x => x.UserId == userId &&
						  x.Value == longSession &&
						  x.FingerPrint == fingerPrint);

			if (savedItem == null)
				return false;

			if (savedItem.ExpiresIn < DateTime.Now)
				return false;

			savedItem.Value = newLongSessionValue;
			savedItem.FingerPrint = newFingerPrint;

			db.Entry(savedItem).State = EntityState.Modified;
			return await db.SaveChangesAsync() > 0;
		}

		public Task<bool> IsLongSessionExistAsync(string userId, string longSession, string fingerPrint)
		{
			 return db.LongSessions.AnyAsync(x => x.UserId == userId &&
									  x.Value == longSession &&
									  x.FingerPrint == fingerPrint);
		}

		public Task<LongSession> GetLongSessionAsync(string value)
		{
			return db.LongSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Value == value);
		}

		public async Task<bool> ConfirmUserRegisterAsync(string userId)
		{
			var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
			if (user == null)
				return false;

			user.EmailConfirmed = true;

			db.Entry(user).State = EntityState.Modified;
			return await db.SaveChangesAsync() > 0;
		}
	}
}
