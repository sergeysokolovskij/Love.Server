using Api.Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPlatforms.Core.Hubs
{
    public class UpdateNotifierHub : BaseHub
    {
        public UpdateNotifierHub(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


    }
}
