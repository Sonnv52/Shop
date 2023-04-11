using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheService.Repository
{
    public interface ICacheService
    {
        public Task ResetProductAsysn(string key);
    }
}
