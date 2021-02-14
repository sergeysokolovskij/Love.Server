using Api.Services.Exceptions;
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

        public ChatService(IChatAcessService chatAcessService)
        {
            this.chatAcessService = chatAcessService;
        }

        public async Task StartChatAsync(string u1, string u2)
        {
            if (!await chatAcessService.HasAcess(u1, u2))
                throw new ApiError(new ServerException("Cannot create chat by policy"));
        }
    }
}
