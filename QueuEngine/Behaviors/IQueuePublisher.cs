using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuEngine.Behaviors
{
    internal interface IQueuePublisher<T>
    {
         Task SendMessageAsync(T message);
        Task SendMessagesAsync(IList<T> messages);  
    }
}
