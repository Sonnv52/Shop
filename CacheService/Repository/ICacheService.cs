using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models;

namespace CacheService.Repository
{
    public interface ICacheService
    {
        public Task<PagingSearch> ResetProductAsysn(string key);
    }
}
