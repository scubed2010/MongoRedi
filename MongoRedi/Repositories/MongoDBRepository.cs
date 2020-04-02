﻿using Microsoft.Extensions.Configuration;
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
using System.Text;

namespace MongoRedi.Repositories
{
    public class MongoDBRepository<TCollection> : IMongoDBRepository<TCollection> where TCollection : BaseCollection
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<TCollection> _collection;
        private readonly IRedisRepository _redisRepository;
        private readonly bool _cache;

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

            _cache = typeof(TCollection).IsDefined(typeof(CacheAttribute), false) && _enableCache;

            if (_cache)
            {
                _redisRepository = new RedisRepository(_redisConnectionString, _redisDatabase);
            }
        }

        public IEnumerable<TCollection> GetAll()
        {
            if (_cache)
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

        public IEnumerable<TCollection> Search(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cache)
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

        public int Count(Expression<Func<TCollection, bool>> predicate)
        {
            if (_cache)
            {
                try
                {
                    if (_redisRepository.Exists<TCollection>())
                    {
                        return _redisRepository.Get<IEnumerable<TCollection>>(typeof(TCollection)).Where(predicate.Compile()).Count();
                    }

                    GetAll();
                }
                catch
                {
                    return _collection.AsQueryable().Where(predicate.Compile()).Count();
                }
            }

            return _collection.AsQueryable().Where(predicate.Compile()).Count();
        }

        public TCollection GetById(ObjectId id)
        {
            if (_cache)
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

        public TCollection GetById(string id)
        {
            return GetById(ObjectId.Parse(id));
        }

        public ObjectId Insert(TCollection collection)
        {
            _collection.InsertOne(collection);

            if (_cache)
            {
                _redisRepository.Delete<TCollection>();
            }

            return collection.Id;
        }

        public void InsertMany(IEnumerable<TCollection> collections)
        {
            _collection.InsertMany(collections);

            if (_cache)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public void Update(ObjectId id, TCollection collection)
        {
            _collection.ReplaceOne(x => x.Id == id, collection);

            if (_cache)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public void Delete(ObjectId id)
        {
            _collection.DeleteOne(x => x.Id == id);

            if (_cache)
            {
                _redisRepository.Delete<TCollection>();
            }
        }

        public void Delete(string id)
        {
            Delete(ObjectId.Parse(id));
        }
    }
}
