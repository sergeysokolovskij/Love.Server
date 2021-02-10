using Api.Dal.Dev;
using Api.DAL.Base;
using Api.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider
{
	public interface IUserDevAccountProvider : IBaseProvider<UserAccount>
	{

	}
	public class UserDevAccountProvider : BaseProvider<UserAccount>, IUserDevAccountProvider
	{
		public UserDevAccountProvider(ApplicationContext dbContext) : base(dbContext)
		{
		}
	}
}
