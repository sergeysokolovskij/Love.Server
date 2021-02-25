using Api.Providers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Filters
{
    public class PhoneConfirmed4AcessFilter :  ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger<PhoneConfirmed4AcessFilter>();
            logger.LogInformation("Try to pass auth filter...");

            var controller = (BaseController)context.Controller;

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = controller.Unauthorized();
                logger.LogInformation("Filter not passed. User is not authenticated");

                return;
            }

            string userName = context.HttpContext.User.Identity.Name;

            var userProvider = context.HttpContext.RequestServices.GetRequiredService<IUserProvider>();
            var currentUser = await userProvider.GetModelBySearchPredicate(x => x.UserName == userName);

            if (!currentUser.PhoneNumberConfirmed)
            {
                context.Result = controller.Unauthorized();
                logger.LogInformation("Filter not passed. Phone doesnt confirm");
                return;
            }

            await next();
        }
    }
}
