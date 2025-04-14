namespace Shared.Services.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T data, TimeSpan? expiration = null);
        bool Remove(string key);
        void Clear();
        bool Contains(string key);
    }
}