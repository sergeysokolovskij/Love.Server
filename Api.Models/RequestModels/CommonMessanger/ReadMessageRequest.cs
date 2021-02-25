using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.RequestModels.CommonMessanger
{
    public class ReadMessageRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("MessageId")]
        public string MessageId { get; set; }
    }
}
