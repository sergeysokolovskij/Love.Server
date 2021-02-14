using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Processing
{
    public class ProcessingProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IServiceScope scope;

        public ProcessingProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }

        public IReadMessagesProcessingHandler readMessagesProcessingHandler
        {
            get
            {
                return scope.ServiceProvider.GetRequiredService<IReadMessagesProcessingHandler>();
            }
        }
    }
}
