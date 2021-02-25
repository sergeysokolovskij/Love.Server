using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Api.Models.RequestModels.MessangerSessionController
{
	public class CreateMessangerSessionRequest
	{
		[Required]
		[JsonProperty("PublicKey")]
		public string PublicKey { get; set; } // публичный ключ для клиента
	}
}
