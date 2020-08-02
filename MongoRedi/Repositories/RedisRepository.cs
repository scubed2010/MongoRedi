using MongoRedi.Converters;
using MongoRedi.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> ExistsAsync<T>()
        {
            var key = $"{typeof(T).Name}";

            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
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

        public async Task<T> GetAsync<T>(Type type)
        {
            var key = $"{type.Name}";

            return JsonConvert.DeserializeObject<T>(await _db.StringGetAsync(key), _serializerSettings);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(await _db.StringGetAsync(key), _serializerSettings);
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

        public async Task SetAsync<T>(Type type, T value)
        {
            var key = $"{type.Name}";

            await _db.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _db.StringSetAsync(key, JsonConvert.SerializeObject(value));
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

        public async Task DeleteAsync<T>()
        {
            var key = $"{typeof(T).Name}";

            await _db.KeyDeleteAsync(key);
        }

        public async Task DeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
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
