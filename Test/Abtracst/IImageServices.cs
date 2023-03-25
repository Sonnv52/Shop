﻿using Shop.Api.Models.ListLog;

namespace Shop.Api.Abtracst
{
    public interface IImageServices
    {
        public Task<ImageLog> AddAsync();
        public Task<ImageLog> RemoveAsync();
    }
}