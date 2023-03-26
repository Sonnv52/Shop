using System.Net;

namespace Shop.Api.Exceptions
{
    public class NotFoundException : ShopException
    {
        public NotFoundException(string message, List<string>? errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message, errors, statusCode)
        {
        }
    }
}
