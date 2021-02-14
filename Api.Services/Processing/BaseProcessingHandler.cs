using Api.Services.Brocker;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Processing
{
    public interface IBaseProcessingHandler
    {
        Task SetHandlerAsync(string message);
    }
    public class BaseProcessingHandler : IBaseProcessingHandler
    {

        protected readonly IConnectionsProvider connectionsProvider;

        public BaseProcessingHandler(IConnectionsProvider connectionsProvider)
        {
            this.connectionsProvider = connectionsProvider;
        }

        public virtual Task SetHandlerAsync(string message)
        {
            throw new NotImplementedException();
        }

       
    }
}
