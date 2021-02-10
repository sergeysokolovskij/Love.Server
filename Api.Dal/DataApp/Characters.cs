using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Characters : IBaseEntity
	{
		public long Id { get; set; }
		public long BaseCharectersId { get; set; }
		public long ProfileId { get; set; }
		public string Lng { get; set; }
		public Profile Profile { get; set; }
		public BaseCharecters BaseCharacters { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}

	public class BaseCharecters
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Lng { get; set; }
	}
}
