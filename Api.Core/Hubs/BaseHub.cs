using Api.DAL;
using Api.Providers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Hubs
{
    public class BaseHub : Hub
    {
        protected readonly IServiceProvider serviceProvider;

        public BaseHub(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            using (var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    var userProvider = scope.ServiceProvider.GetRequiredService<IUserProvider>();
                    var result = await userProvider.GetModelBySearchPredicate(x => x.UserName == Context.User.Identity.Name);

                    return result;
                }
                return null;
            }
        }
    }
}
