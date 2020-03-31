using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRedi.Attributes;
using MongoRedi.Interfaces;
using MongoRedi.Models;
using System;
using System.Collections.Generic;
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
        private readonly bool _cache;
        //private readonly CacheManager _cacheManager;

        private readonly string _mongoDBConnectionString;
        private readonly string _mongoDBDatabase;
        private readonly bool _enableCache;

        public MongoDBRepository()
        {
            #region Initialize
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            _mongoDBConnectionString = configuration["MongoRedi:MongoDBConnectionString"];
            _mongoDBDatabase = configuration["MongoRedi:MongoDBDatabase"];
            _enableCache = Convert.ToBoolean(configuration["MongoRedi:Cache"]);
            #endregion

            _mongoClient = new MongoClient(_mongoDBConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(_mongoDBDatabase);

            var collectionName =
                (typeof(TCollection).GetCustomAttributes(typeof(CollectionNameAttribute), true)[0] as CollectionNameAttribute).CollectionName;
            _collection = _mongoDatabase.GetCollection<TCollection>(collectionName);

            _cache = typeof(TCollection).IsDefined(typeof(CacheAttribute), false) && Convert.ToBoolean(configuration["Cache"]);

            //if (_cache)
            //{
            //    _cacheManager = new CacheManager(configuration);
            //}
        }

        public IEnumerable<TCollection> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }
        public IEnumerable<TCollection> Search(Expression<Func<TCollection, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate.Compile()).ToList();
        }

        public int Count(Expression<Func<TCollection, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate.Compile()).Count();
        }

        public TCollection GetById(ObjectId id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public TCollection GetById(string id)
        {
            return _collection.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefault();
        }

        public ObjectId Insert(TCollection collection)
        {
            _collection.InsertOne(collection);

            return collection.Id;
        }

        public void InsertMany(IEnumerable<TCollection> collections)
        {
            _collection.InsertMany(collections);
        }

        public void Update(ObjectId id, TCollection collection)
        {
            _collection.ReplaceOne(x => x.Id == id, collection);
        }

        public void Delete(ObjectId id)
        {
            _collection.DeleteOne(x => x.Id == id);
        }

        public void Delete(string id)
        {
            _collection.DeleteOne(x => x.Id == ObjectId.Parse(id));
        }
    }
}
