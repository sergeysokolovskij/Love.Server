using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Blockchain
{
	public class Block
	{
		public string Data { get; set; }
		public DateTime CreatedOn { get; set; }
		public string Hash { get; set; }
		public string PreviousHash { get; set; }
		public string Algoritm { get; set; }
		public string User { get; set; }
		public string Version { get; set; }
	}
}
