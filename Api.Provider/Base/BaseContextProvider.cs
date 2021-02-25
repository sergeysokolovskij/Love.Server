using Api.DAL.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider.Base
{
	public class BaseContextProvider
	{
		protected ApplicationContext db;

		public BaseContextProvider(ApplicationContext db)
		{
			this.db = db;
		}
	}
}
