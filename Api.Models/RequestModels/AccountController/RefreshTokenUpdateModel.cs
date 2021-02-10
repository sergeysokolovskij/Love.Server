using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.RequestModels.AccountController
{
	public class RefreshTokenUpdateModel
	{
		[JsonProperty("refreshToken")]
		public string RefreshToken { get; set; }
	}
}
