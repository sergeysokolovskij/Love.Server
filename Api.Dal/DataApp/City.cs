using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class City // имеется ввиду населенный пункт
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long CountryId { get; set; }
		public Country Country { get; set; }
		public List<Profile> Profiles { get; set; }
	}
}
