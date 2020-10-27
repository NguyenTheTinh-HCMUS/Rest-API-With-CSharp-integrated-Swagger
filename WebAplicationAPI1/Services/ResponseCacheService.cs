using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task CachedResponseAsync(string cacheKey, object response, TimeSpan TimeToLive)
        {
            if (response == null)
            {
                return;
            }
            var serializeResponse = JsonConvert.SerializeObject(response);
            await _distributedCache.SetStringAsync(cacheKey, serializeResponse, new DistributedCacheEntryOptions { 
            
                    AbsoluteExpirationRelativeToNow=TimeToLive
            });
        }

        public async Task<string> GetCachedResonseAsync(string cacheKey)
        {
            var cacheResonse = await _distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cacheResonse) ? null : cacheResonse;
        }
    }
}
