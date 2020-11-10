using Api.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Options
{
	public static class DiExtensions
	{
		public static void OptionsConfigure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions();
			services.Configure<AuthOptions>(configuration.GetSection("Auth"));
			services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));
			services.Configure<TokenLifeTimeOptions>(configuration.GetSection("TokenOptions:TokenLifeTimeOptions"));
			services.Configure<MailOptions>(configuration.GetSection("Mail"));
			services.Configure<UrlPathOptions>(configuration.GetSection("UrlPathes"));
			services.Configure<ViewOptions>(configuration.GetSection("ViewOptions"));
			services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
		}
	}
}
