using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Api.Models;

namespace CacheService.Repository
{
    public interface ICacheService
    {
        public Task ResetProductAsysn(string key);
    }
}
