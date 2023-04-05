using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuEngine.Behaviors
{
    public interface IQueueSubscriber
    {
        Task ProcessQueue();
    }
}
