using Api.Provider;
using Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Crypt.MessangerCryptors
{
    public interface IStandartAesCryptor
    {
        Task<string> CryptAsync(string userId ,string data);
        Task<string> DecryptAsync(string userId, string data);
    }
    public class StandartCryptor : IStandartAesCryptor
    {
        private readonly IAesCipher aes;
        private readonly IStrongKeyProvider strongKeyProvider;
        public StandartCryptor(
            IStrongKeyProvider strongKeyProvider,
            IAesCipher aes
            )
        {
            this.strongKeyProvider = strongKeyProvider;
            this.aes = aes;
        }


        public async Task<string> CryptAsync(string userId, string data)
        {
            var cypher = await strongKeyProvider.GetStrongKeyAsync(userId);
            var result = aes.Crypt(cypher.Secret.ToUrlSafeBase64(), data);

            return result;
        }

        public async Task<string> DecryptAsync(string userId, string data)
        {
            var cypher = await strongKeyProvider.GetStrongKeyAsync(userId);
            var result = aes.Decrypt(cypher.Secret.ToUrlSafeBase64(), data);

            return result;
        }
    }
}
