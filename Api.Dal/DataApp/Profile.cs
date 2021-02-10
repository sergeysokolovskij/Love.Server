using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Profile : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public long CountryId { get; set; }
		public long CityId { get; set; }
		public int Age { get; set; }
		public List<Picture> Pictures { get; set; }
		public List<Characters> Characters { get; set; }
		public User User { get; set; }
		public Country Country { get; set; }
		public City City { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
