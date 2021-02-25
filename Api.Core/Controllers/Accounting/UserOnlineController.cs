using Api.Core.Filters;
using Api.Dal.Accounting;
using Api.Models.RequestModels.UserOnlineController;
using Api.Provider.Accounting;
using Api.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Controllers.Accounting
{
    [Route("[controller]")]
    [ApiController]
    public class UserOnlineController : BaseController
    {

        private readonly IUserOnlineAccountingProvider userOnlineAccountingProvider;

        public UserOnlineController(IServiceProvider serviceProvider, 
            IUserProvider userProvider,
            IUserOnlineAccountingProvider userOnlineAccountingProvider) : base(serviceProvider, userProvider)
        {
            this.userOnlineAccountingProvider = userOnlineAccountingProvider;
        }

        [Authorize(Roles = "ProtocoledUsers")]
        [HttpPost]
        public async Task<IActionResult> UserOnlineAsync([FromBody]UserOnlinePostRequest model)
        {
            var user = await GetUserAsync();
            await userOnlineAccountingProvider.CreateOrUpdateAsync(new UserOnlineAccounting() 
            {
                IsOnline = model.IsOnline,
                UserId = user.Id
            });

            return Created();
        }


        [Authorize(Roles = "ProtocoledUsers")]
        [HttpGet]
        public async Task<IActionResult> GetLastUserOnlineAsync()
        {
            var user = await GetUserAsync();
            var result = await userOnlineAccountingProvider.GetLastUserVisitAsync(user.Id);

            return Ok(new
            {
                lastDate = result
            });
        }


        [PhoneConfirmed4AcessFilter]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
