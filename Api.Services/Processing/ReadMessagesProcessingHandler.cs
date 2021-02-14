using Api.Factories;
using Api.Services.Brocker;
using Api.Services.Messanger;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Processing
{
    public interface IReadMessagesProcessingHandler
    {
        Task HandleAsync(string sessionId);
    }

    public class ReadMessagesProcessingHandler : BaseProcessingHandler, IReadMessagesProcessingHandler
    {

        private readonly IMessangerService messangerService;

        public ReadMessagesProcessingHandler(IConnectionsProvider connectionsProvider,
            IMessangerService messangerService) : base(connectionsProvider)
        {
            this.messangerService = messangerService;
        }

        public Task HandleAsync(string sessionId)
        {
            var connection = connectionsProvider.GetConnection().CreateModel();

            string routingKey = BrockerKeysFactory.GenerateQueueKey(sessionId, BrcokerKeysTypes.readmessage);
            try 
            {
                connection.QueueBind(routingKey, "readedmessages", routingKey); // пока не нашел вменяемого способа проверить существует ли очередь
            }
            catch
            {
                connection.QueueDeclare(routingKey, durable: true, exclusive: false, false, new Dictionary<string, object>()
                {
                    {"x-queue-type","classic" }
                });
                connection.QueueBind(routingKey, "readedmessages", "");
            }
           
            var subscriber = new AsyncEventingBasicConsumer(connection);

            subscriber.Received += async (conn, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await messangerService.ReadMessageAsync(message);
            };

            connection.BasicConsume(routingKey, true, subscriber);

            return Task.CompletedTask;
        }
    }
}
