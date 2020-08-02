using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRedi.Attributes;
using MongoRedi.Interfaces;
using MongoRedi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MongoRedi.Repositories
{
    public class MongoDBRepository<TCollection> : IMongoDBRepository<TCollection> where TCollection : BaseCollection
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<TCollection> _collection;
        private readonly IRedisRepository _redisRepository;
        private readonly bool _cacheCollection;
        private readonly bool _cacheDocument;

        private readonly string _mongoDBConnectionString;
        private readonly string _mongoDBDatabase;
        private readonly string _redisConnectionString;
        private readonly string _redisDatabase;
        private readonly bool _enableCache;

        public MongoDBRepository()
        {
            #region Initialize
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            if (configuration.GetChildren().Count() > 0)
            {
                _mongoDBConnectionString = configuration["MongoRedi:MongoDBConnectionString"];
                _mongoDBDatabase = configuration["MongoRedi:MongoDBDatabase"];

                _redisConnectionString = configuration["MongoRedi:RedisConnectionString"];
                _redisDatabase = configuration["MongoRedi:RedisDatabase"];

                _enableCache = Convert.ToBoolean(configuration["MongoRedi:Cache"]);
            }
            else
            {
                _mongoDBConnectionString = ConfigurationManager.AppSettings["MongoRedi_MongoDBConnectionString"];
                _mongoDBDatabase = ConfigurationManager.AppSettings["MongoRedi_MongoDBDatabase"];

                _redisConnectionString = ConfigurationManager.AppSettings["MongoRedi_RedisConnectionString"];
                _redisDatabase = ConfigurationManager.AppSettings["MongoRedi_RedisDatabase"];

                _enableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["MongoRedi_Cache"]);
            }
            #endregion

            _mongoClient = new MongoClient(_mongoDBConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(_mongoDBDatabase);

            var collectionName =
                (typeof(TCollection).GetCustomAttributes(typeof(CollectionNameAttribute), true)[0] as CollectionNameAttribute).CollectionName;
            _collection = _mongoDatabase.GetCollection<TCollection>(collectionName);

            _cacheCollection = typeof(TCollection).IsDefined(typeof(CacheCollectionAttribute), false) && _enableCache;
            _cacheDocument = typeof(TCollection).IsDefined(typeof(CacheDocumentAttribute), false) && _enableCache;

            if (_cacheCollection || _cacheDocument)
            {
                _redisRepository = new RedisRepository(_redisConnectionString, _redisDatabase);
            }
        }

        public IEnumerable<TCollection> GetAll()
        {
            if (_cacheCollection)
            {
                try
                {
                    if (_redisRepository.Exists<TCollection>())
                    {
                        return _redisRepository.Get<IEnumerable<TCollection>>(typeof(TCollection));
                    }

                    var data = _collection.Find(_ => true).ToList();
                    _redisRepository.Set(typeof(TCollection), data);

                    return data;
                }
                catch
                {
                    return _collection.Find(_ => true).ToList();
                }
            }

            return _collection.Find(_ => true).ToList();
        }

        public async Task<IEnumerable<TCollection>> GetAllAsync()
        {
            if (_cacheCollection)
            {
                try
                {
                    if (await _redisRepository.ExistsAsync<TCollection>())
                    {
                        return await _redisRepository.GetAsync<IEnumerable<TCollection>>(typeof(TCollection));
                    }

                    var data = await _collection.FindAsync(_ => true).Result.ToListAsync();
                    await _redisRepository.SetAsync(typeof(TCollection), data);

                    return data;
                }
                catch
                {
                    return await _collection.FindAsync(_ => true).Result.ToListAsync();
                }
            }

            return await _collection.FindAsync(_ => true).Result.ToListAsync();
        }

        public IEnumerable<TCollection> Search(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cacheCollection)
            {
                try
                {
                    if (_redisRepository.Exists<TCollection>())
                    {
                        var cachedResult = _redisRepository.Get<IEnumerable<TCollection>>(typeof(TCollection)).Where(predicate.Compile()).ToList();

                        if (cachedResult != null)
                        {
                            return cachedResult;
                        }
                    }

                    GetAll();
                }
                catch
                {
                    return _collection.AsQueryable().Where(predicate.Compile()).ToList();
                }

            }

            return _collection.AsQueryable().Where(predicate.Compile()).ToList();
        }

        public async Task<IEnumerable<TCollection>> SearchAsync(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cacheCollection)
            {
                try
                {
                    if (await _redisRepository.ExistsAsync<TCollection>())
                    {
                        var cachedResult = await _redisRepository.GetAsync<IEnumerable<TCollection>>(typeof(TCollection));

                        if (cachedResult != null)
                        {
                            return cachedResult.Where(predicate.Compile()).ToList();
                        }
                    }

                    GetAll();
                }
                catch
                {
                    return _collection.AsQueryable().Where(predicate.Compile()).ToList();
                }

            }

            return _collection.AsQueryable().Where(predicate.Compile()).ToList();
        }

        public int Count(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cacheCollection)
            {
                try
                {
                    if (_redisRepository.Exists<TCollection>())
                    {
                        return _redisRepository.Get<IEnumerable<TCollection>>(typeof(TCollection)).Count(predicate.Compile());
                    }

                    GetAll();
                }
                catch
                {
                    return _collection.AsQueryable().Count(predicate.Compile());
                }
            }

            return _collection.AsQueryable().Count(predicate.Compile());
        }

        public async Task<int> CountAsync(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cacheCollection)
            {
                try
                {
                    if (await _redisRepository.ExistsAsync<TCollection>())
                    {
                        var result = await _redisRepository.GetAsync<IEnumerable<TCollection>>(typeof(TCollection));

                        return result.Count(predicate.Compile());
                    }

                    await GetAllAsync();
                }
                catch
                {
                    return _collection.AsQueryable().Count(predicate.Compile());
                }
            }

            return _collection.AsQueryable().Count(predicate.Compile());
        }

        public TCollection GetById(ObjectId id)
        {
            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                if (_redisRepository.Exists(key))
                {
                    return _redisRepository.Get<TCollection>(key);
                }
                else
                {
                    var data = _collection.Find(x => x.Id == id).FirstOrDefault();

                    if (data != null)
                    {
                        _redisRepository.Set(key, data);
                    }
                    
                    return data;
                }
            }

            if (_cacheCollection)
            {
                try
                {
                    if (_redisRepository.Exists<TCollection>())
                    {
                        var cachedResult = _redisRepository.Get<IEnumerable<TCollection>>(typeof(TCollection)).FirstOrDefault(x => x.Id == id);

                        if (cachedResult != null)
                        {
                            return cachedResult;
                        }
                    }

                    GetAll();
                }
                catch
                {
                    return _collection.Find(x => x.Id == id).FirstOrDefault();
                }
            }

            return _collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public async Task<TCollection> GetByIdAsync(ObjectId id)
        {
            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                if (await _redisRepository.ExistsAsync(key))
                {
                    return await _redisRepository.GetAsync<TCollection>(key);
                }
                else
                {
                    var data = await _collection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

                    if (data != null)
                    {
                        await _redisRepository.SetAsync(key, data);
                    }

                    return data;
                }
            }

            if (_cacheCollection)
            {
                try
                {
                    if (await _redisRepository.ExistsAsync<TCollection>())
                    {
                        var cachedResult = await _redisRepository.GetAsync<IEnumerable<TCollection>>(typeof(TCollection));

                        if (cachedResult != null)
                        {
                            return cachedResult.FirstOrDefault(x => x.Id == id);
                        }
                    }

                    await GetAllAsync();
                }
                catch
                {
                    return await _collection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();
                }
            }

            return await _collection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();
        }

        public TCollection GetById(string id)
        {
            return GetById(ObjectId.Parse(id));
        }

        public async Task<TCollection> GetByIdAsync(string id)
        {
            return await GetByIdAsync(ObjectId.Parse(id));
        }

        public ObjectId Insert(TCollection collection)
        {
            _collection.InsertOne(collection);

            if (_cacheCollection)
            {
                _redisRepository.Delete<TCollection>();
            }

            return collection.Id;
        }

        public async Task<ObjectId> InsertAsync(TCollection collection)
        {
            await _collection.InsertOneAsync(collection);

            if (_cacheCollection)
            {
                await _redisRepository.DeleteAsync<TCollection>();
            }

            return collection.Id;
        }

        public void InsertMany(IEnumerable<TCollection> collections)
        {
            _collection.InsertMany(collections);

            if (_cacheCollection)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public async Task InsertManyAsync(IEnumerable<TCollection> collections)
        {
            await _collection.InsertManyAsync(collections);

            if (_cacheCollection)
            {
                await _redisRepository.DeleteAsync<TCollection>();
            }
        }

        public void Update(ObjectId id, TCollection collection)
        {
            _collection.ReplaceOne(x => x.Id == id, collection);

            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                _redisRepository.Delete(key);
            }

            if (_cacheCollection)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public async Task UpdateAsync(ObjectId id, TCollection collection)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, collection);

            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                await _redisRepository.DeleteAsync(key);
            }

            if (_cacheCollection)
            {
                await _redisRepository.DeleteAsync<TCollection>();
            }
        }

        public void Delete(ObjectId id)
        {
            _collection.DeleteOne(x => x.Id == id);

            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                _redisRepository.Delete(key);
            }

            if (_cacheCollection)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);

            if (_cacheDocument)
            {
                string key = $"{typeof(TCollection).Name}_{id}";

                await _redisRepository.DeleteAsync(key);
            }

            if (_cacheCollection)
            {
                await _redisRepository.DeleteAsync<TCollection>();
            }
        }

        public void Delete(string id)
        {
            Delete(ObjectId.Parse(id));
        }

        public async Task DeleteAsync(string id)
        {
            await DeleteAsync(ObjectId.Parse(id));
        }
    }
}
