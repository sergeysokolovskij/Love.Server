using Api.Services.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Messanger
{
    public interface IChatService
    {
        Task StartChatAsync(string u1, string u2);
    }

    public class ChatService : IChatService
    {
        private readonly IChatAcessService chatAcessService;
        private readonly ILogger logger;

        public ChatService(IChatAcessService chatAcessService,
            ILoggerFactory loggerFactory)
        {
            this.chatAcessService = chatAcessService;
            logger = loggerFactory.CreateLogger<ChatService>();
        }

        public async Task StartChatAsync(string u1, string u2)
        {
            if (!await chatAcessService.HasAcess(u1, u2))
            {
                logger.LogInformation($"Cannot create chat becouse account policy. Was tryed by u1:{u1} and u2: {u2}");
                throw new ApiError(new ServerException("Cannot create chat by policy"));
            }

        }
    }
}
