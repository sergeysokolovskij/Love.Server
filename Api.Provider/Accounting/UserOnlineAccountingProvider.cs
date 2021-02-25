using Api.Dal.Accounting;
using Api.DAL.Base;
using Api.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider.Accounting
{
    public interface IUserOnlineAccountingProvider : IBaseProvider<UserOnlineAccounting>
    {
        Task<DateTime> GetLastUserVisitAsync(string userId);
    }
    public class UserOnlineAccountingProvider : BaseProvider<UserOnlineAccounting>, IUserOnlineAccountingProvider
    {
        public UserOnlineAccountingProvider(ApplicationContext db) : base(db)
        {
        }

        public async Task<DateTime> GetLastUserVisitAsync(string userId)
        {
            var visit = await db.UserOnlineAccountings
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            return visit.Created;
        }
    }
}
