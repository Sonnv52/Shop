using MassTransit;
using Microsoft.Extensions.Hosting;
using Shop.Api.Models;
using System.Reflection;

static void Main(string[] args)
{
    Console.WriteLine("haha");
    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("order-submitted", e =>
    {
        e.Consumer<OrderSubmittedConsumer>();
    });
    });

    busControl.Start();
    Console.WriteLine("Consumer started. Press any key to exit.");
    Console.ReadKey();
    busControl.Stop();
}

public class OrderSubmittedConsumer : IConsumer<Tesstttt>
{
    public async Task Consume(ConsumeContext<Tesstttt> context)
    {
        Console.WriteLine($"Received order submitted: OrderId = {context.Message.message}");
    }

}