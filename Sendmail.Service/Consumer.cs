using MassTransit;
using Share.Message;

namespace Sendmail.Service
{
    public class Consumer : IConsumer<ProductSend>
    {
        public async Task Consume(ConsumeContext<ProductSend> context)
        {
            try
            {
                var product = context.Message;
                Console.WriteLine($"Received product: {product.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}

