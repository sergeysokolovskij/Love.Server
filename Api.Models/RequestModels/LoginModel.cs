using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ShopPlatform.Models.RequestModels
{
	public class LoginModel
	{
		[Required]
		[FromForm(Name = "userName")]
		public string UserName { get; set; }

		[Required]
		[FromForm(Name = "password")]
		public string Password { get; set; }
	}
}
