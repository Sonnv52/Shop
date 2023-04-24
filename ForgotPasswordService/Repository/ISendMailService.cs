using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgotPasswordService.Message;

namespace ForgotPasswordService.Repository
{
    public interface ISendMailService<T> where T : class
    {
     public Task SendAsync(T message);
    }
}
