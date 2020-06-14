using System;
using System.Collections.Generic;
using System.Text;

namespace MongoRedi.Interfaces
{
    public interface IRedisRepository
    {
        bool Exists<T>();
        bool Exists(string key);
        T Get<T>(Type type);
        T Get<T>(string key);
        void Set<T>(Type type, T value);
        void Set<T>(string key, T value);
        void Delete<T>();
        void Delete(string key);
        List<string> GetKeys(string pattern);
    }
}
