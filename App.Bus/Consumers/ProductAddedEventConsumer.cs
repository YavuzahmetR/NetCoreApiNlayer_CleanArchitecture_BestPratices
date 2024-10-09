using App.Domain.Events;
using MassTransit;

namespace App.Bus.Consumers
{
    public class ProductAddedEventConsumer() : IConsumer<ProductAddedEvent>
    {
        public Task Consume(ConsumeContext<ProductAddedEvent> context)
        {

            //rabbitMQ or any queue system waits for binary formatted data. MassTransit serialize - deserialize datas for you.
            Console.WriteLine($"Gelen Event: {context.Message.Id} - {context.Message.Name} - {context.Message.Price}");

            return Task.CompletedTask;
        }
    }
}
