using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Accounting
{
    public class UserOnlineAccounting : IBaseEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public bool IsOnline { get; set; }
        public User User { get; set; } 
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
