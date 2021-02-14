using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Messanger
{
    public interface IChatAcessService
    {
        Task<bool> HasAcess(string u1, string u2); 
    }
    public class ChatAcessService : IChatAcessService
    {
        public Task<bool> HasAcess(string u1, string u2)
        {
            return Task.FromResult(true);
        }
    }
}
