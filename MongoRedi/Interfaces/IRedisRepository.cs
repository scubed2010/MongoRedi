using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRedi.Interfaces
{
    public interface IRedisRepository
    {
        bool Exists<T>();
        bool Exists(string key);
        Task<bool> ExistsAsync<T>();
        Task<bool> ExistsAsync(string key);
        T Get<T>(Type type);
        T Get<T>(string key);
        Task<T> GetAsync<T>(Type type);
        Task<T> GetAsync<T>(string key);
        void Set<T>(Type type, T value);
        void Set<T>(string key, T value);
        Task SetAsync<T>(Type type, T value);
        Task SetAsync<T>(string key, T value);
        void Delete<T>();
        void Delete(string key);
        Task DeleteAsync<T>();
        Task DeleteAsync(string key);
        List<string> GetKeys(string pattern);
    }
}
