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

                // Xử lý tin nhắn tại đây
                Console.WriteLine($"Received product: {product.Name}");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}

