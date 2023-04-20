using MassTransit;
using Share.Message;
using Shop.Api.Abtracst;
using Shop.Api.Data;

namespace Shop.Api.Repository
{
    public class PushlishResponsitory : IPushlishService<ProductSend>
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;
        private readonly string _exchangeName;
        public PushlishResponsitory(IBus bus, ILogger<PushlishResponsitory> logger) { 
            _bus = bus;
            _logger = logger;
            _exchangeName = "cart";
        }
        public async Task PushlishAsync(ProductSend data)
        {
            if (data is not null)
            {
                Uri uri = new Uri($"rabbitmq://localhost/{_exchangeName}");
                var endPoint = await _bus.GetSendEndpoint(uri);

                try
                {
                    await endPoint.Send(data);
                    _logger.LogInformation($"Message with id {data.Email} sent successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send message with id {data.Email}.");
                    throw;
                }
            }
        }
    }
}
