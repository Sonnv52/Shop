using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgotPasswordService.Repository
{
    public class Class1 : IAccount
    {
        public async Task<string> GetAccount(string accountId)
        {
            return "Son";
        }
    }
}
