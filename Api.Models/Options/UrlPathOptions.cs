using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Options
{

	public class UrlPathOptions
	{
		public string Site { get; set; }
		public string Api { get; set; }

		public bool IsHttps // для правильной установки кук. 
		{
			get
			{
				return GetSchemaAndHostApi().schema == "https";
			}
		}

		public (string schema, string host) GetSchemaAndHostApi()
		{
			if (Api.StartsWith("http://"))
				return ("http", Api.Substring(7));
			if (Api.StartsWith("https://"))
				return ("https", Api.Substring(8));
			throw new Exception("Incorrect data. Site must start with http/https");
		}
	}
}
