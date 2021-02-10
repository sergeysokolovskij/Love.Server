using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Messanger
{
	public interface IMessangerBrockerService
	{
		void SubscribeToAll();
	}
	public class MessangerBrockerService : IMessangerBrockerService
	{
		public void SubscribeToAll()
		{
			throw new NotImplementedException();
		}
	}
}
