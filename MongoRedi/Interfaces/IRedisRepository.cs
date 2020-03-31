using System;
using System.Collections.Generic;
using System.Text;

namespace MongoRedi.Interfaces
{
    public interface IRedisRepository
    {
        bool Exists<T>();
        T Get<T>(Type type);
        void Set<T>(Type type, T value);
        void Delete<T>();
    }
}
