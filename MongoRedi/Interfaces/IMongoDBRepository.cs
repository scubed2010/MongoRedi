using MongoDB.Bson;
using MongoRedi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MongoRedi.Interfaces
{
    public interface IMongoDBRepository<TCollection> where TCollection : BaseCollection
    {
        IEnumerable<TCollection> GetAll();
        IEnumerable<TCollection> Search(Expression<Func<TCollection, bool>> predicate);
        int Count(Expression<Func<TCollection, bool>> predicate);
        TCollection GetById(ObjectId id);
        TCollection GetById(string id);
        ObjectId Insert(TCollection collection);
        void InsertMany(IEnumerable<TCollection> collections);
        void Update(ObjectId id, TCollection collection);
        void Delete(ObjectId id);
        void Delete(string id);
    }
}
