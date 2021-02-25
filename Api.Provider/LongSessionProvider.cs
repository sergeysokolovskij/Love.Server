using Api.DAL;
using Api.DAL.Base;
using Api.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider
{
	public interface ILongSessionProvider : IBaseProvider<LongSession>
	{

	}
	public class LongSessionProvider : BaseProvider<LongSession>, ILongSessionProvider
	{
		public LongSessionProvider(ApplicationContext db) : base(db)
		{
		}


	}
}
