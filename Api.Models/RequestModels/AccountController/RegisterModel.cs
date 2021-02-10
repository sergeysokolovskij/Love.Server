using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPlatform.Models.RequestModels
{
	public class RegisterModel
	{
		[FromForm(Name ="email")]
		public string Email { get; set; }
		[FromForm(Name = "userName")]
		public string UserName { get; set; }
		[FromForm(Name = "password")]
		public string Password { get; set; }
	}
}	
