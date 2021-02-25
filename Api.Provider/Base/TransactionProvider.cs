using Api.DAL.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider.Base
{
	public interface ITransactionProvider 
	{
		Task<IDbContextTransaction> BeginTransactionAsync();
	}
	public class TransactionProvider :BaseContextProvider, ITransactionProvider
	{

		public TransactionProvider(ApplicationContext db): base(db)
		{
		}

		public Task<IDbContextTransaction> BeginTransactionAsync()
		{	
			return db.Database.BeginTransactionAsync();
		}
	}
}
