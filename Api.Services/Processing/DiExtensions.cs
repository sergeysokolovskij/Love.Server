using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Processing
{
    public static class DiExtensions
    {
        public static void AddProcessingServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IReadMessagesProcessingHandler, ReadMessagesProcessingHandler>();
            serviceCollection.AddScoped<ProcessingProvider>();
        }
    }
}
