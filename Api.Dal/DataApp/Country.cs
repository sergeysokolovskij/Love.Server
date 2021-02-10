using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Country : IBaseEntity
	{
		public long Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public string Name { get; set; }

		public List<Profile> Profiles { get; set; }
		public List<City> Cities { get; set; }
	}
}
