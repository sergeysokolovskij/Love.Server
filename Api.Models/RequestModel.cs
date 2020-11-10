using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
	public class RequestModel
	{
		public Dictionary<string, string> CustomFields = new Dictionary<string, string>();
		[FromForm(Name = "files")]
		public List<IFormFile> Files { get; set; }
		[Required]
		[EmailAddress]
		[FromForm(Name = "email")]
		public string Email { get; set; }
	}
}
