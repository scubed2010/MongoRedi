using MongoRedi.Converters;
using MongoRedi.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace MongoRedi.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly JsonSerializerSettings _serializerSettings;

        public RedisRepository(string redisConnectionString, string redisDatabase)
        {
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _db = _redis.GetDatabase(Convert.ToInt32(redisDatabase));

            _serializerSettings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new ObjectIdConverter() }
            };
        }

        public bool Exists<T>()
        {
            var key = $"{typeof(T).Name}";

            return _db.KeyExists(key);
        }

        public T Get<T>(Type type)
        {
            var key = $"{type.Name}";

            return JsonConvert.DeserializeObject<T>(_db.StringGet(key), _serializerSettings);
        }

        public void Set<T>(Type type, T value)
        {
            var key = $"{type.Name}";

            _db.StringSet(key, JsonConvert.SerializeObject(value));
        }

        public void Delete<T>()
        {
            var key = $"{typeof(T).Name}";

            _db.KeyDelete(key);
        }
    }
}
