using Api.Dal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.DAL
{
	public class Cypher : IBaseEntity
	{
		public long Id { get; set; }
		public byte[] Secret { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
