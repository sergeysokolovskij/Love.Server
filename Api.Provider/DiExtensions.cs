using Api.Provider;
using Api.Provider.Accounting;
using Api.Provider.Base;
using Api.Provider.Cache;
using Api.Provider.Messanger;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Providers
{
	public static class DiExtensions
	{
		public static void RegisterProviders(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<ITransactionProvider, TransactionProvider>();

			serviceCollection.AddScoped<IUserProvider, UserProvider>();
			serviceCollection.AddScoped<ILongSessionProvider, LongSessionProvider>();
			serviceCollection.AddScoped<ICypherProvider, CypherProvider>();
			serviceCollection.AddScoped<IMessageProvider, MessageProvider>();
			serviceCollection.AddScoped<IUserDevAccountProvider, UserDevAccountProvider>();
			serviceCollection.AddScoped<IUserTokenProvider, UserTokenProvider>();
			serviceCollection.AddScoped<ISessionProvider, SessionProvider>();
			serviceCollection.AddScoped<IConnectionProvider, ConnectionProvider>();
			serviceCollection.AddScoped<IStrongKeyProvider, StrongKeyProvider>();
			serviceCollection.AddScoped<IDialogProvider, DialogProvider>();

			serviceCollection.AddSingleton<IBaseRedisProvider, BaseRedisProvider>();
			serviceCollection.AddScoped<IRedisPublisher, RedisPublisher>();

			serviceCollection.RegisterAccounting();
		}
	}
}
