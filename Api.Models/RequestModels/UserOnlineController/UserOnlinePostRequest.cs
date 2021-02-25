using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Api.Models.RequestModels.UserOnlineController
{
    public class UserOnlinePostRequest
    {
        [Required]
        public bool IsOnline { get; set; }
    }
}
