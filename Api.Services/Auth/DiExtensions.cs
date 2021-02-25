
using Api.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Api.Models.Options;

namespace Api.Services.Auth
{
	public static class DiExtensions
	{
        public static void AddAuthService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<IJwtService, JwtService>();

            serviceCollection.AddSingleton<IJwtSigningEncodingKey>(services =>
            {
                var options = services.GetRequiredService<IOptions<TokenOptions>>();
                return new SignInSymmetricKey(options.Value.Key); //ключ для подписиы
            });

            serviceCollection.AddSingleton<IJwtEncryptingEncodingKey>(services =>
            {
                var options = services.GetRequiredService<IOptions<TokenOptions>>();
                return new JwtCrypt(options.Value.CypherKey); // aes-ключ для шифрования 
            });
       
        }
	}
}
