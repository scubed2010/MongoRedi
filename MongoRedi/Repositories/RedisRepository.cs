using MongoRedi.Converters;
using MongoRedi.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoRedi.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IServer _server;
        private readonly IDatabase _db;
        private readonly JsonSerializerSettings _serializerSettings;

        public RedisRepository(string redisConnectionString, string redisDatabase)
        {
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _server = _redis.GetServer(_redis.GetEndPoints().FirstOrDefault());
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

        public bool Exists(string key)
        {
            return _db.KeyExists(key);
        }

        public T Get<T>(Type type)
        {
            var key = $"{type.Name}";

            return JsonConvert.DeserializeObject<T>(_db.StringGet(key), _serializerSettings);
        }

        public T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(_db.StringGet(key), _serializerSettings);
        }

        public void Set<T>(Type type, T value)
        {
            var key = $"{type.Name}";

            _db.StringSet(key, JsonConvert.SerializeObject(value));
        }

        public void Set<T>(string key, T value)
        {
            _db.StringSet(key, JsonConvert.SerializeObject(value));
        }

        public void Delete<T>()
        {
            var key = $"{typeof(T).Name}";

            _db.KeyDelete(key);
        }

        public void Delete(string key)
        {
            _db.KeyDelete(key);
        }

        public List<string> GetKeys(string pattern)
        {
            var keyList = new List<string>();

            foreach (var key in _server.Keys(pattern: $"*{pattern}*", database: _db.Database))
            {
                keyList.Add(key.ToString());
            }

            return keyList;
        }
    }
}
