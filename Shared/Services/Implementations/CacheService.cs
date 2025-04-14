using System.Collections.Concurrent;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        
        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var cacheItem) && !cacheItem.IsExpired)
            {
                return (T)cacheItem.Value;
            }
            
            // Remove expired items
            if (cacheItem != null && cacheItem.IsExpired)
            {
                _cache.TryRemove(key, out _);
            }
            
            return default;
        }
        
        public void Set<T>(string key, T data, TimeSpan? expiration = null)
        {
            var expirationTime = expiration.HasValue ? 
                DateTime.UtcNow.Add(expiration.Value) : DateTime.UtcNow.AddDays(1);
                
            var cacheItem = new CacheItem
            {
                Value = data,
                ExpirationTime = expirationTime
            };
            
            _cache.AddOrUpdate(key, cacheItem, (k, oldValue) => cacheItem);
        }
        
        public bool Remove(string key)
        {
            return _cache.TryRemove(key, out _);
        }
        
        public void Clear()
        {
            _cache.Clear();
        }
        
        public bool Contains(string key)
        {
            return _cache.TryGetValue(key, out var cacheItem) && !cacheItem.IsExpired;
        }
        
        private class CacheItem
        {
            public object Value { get; set; }
            public DateTime ExpirationTime { get; set; }
            public bool IsExpired => DateTime.UtcNow > ExpirationTime;
        }
    }
}