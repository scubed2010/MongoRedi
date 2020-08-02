using MongoDB.Bson;
using MongoRedi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRedi.Interfaces
{
    public interface IMongoDBRepository<TCollection> where TCollection : BaseCollection
    {
        IEnumerable<TCollection> GetAll();
        Task<IEnumerable<TCollection>> GetAllAsync();
        IEnumerable<TCollection> Search(Expression<Func<TCollection, bool>> predicate);
        Task<IEnumerable<TCollection>> SearchAsync(Expression<Func<TCollection, bool>> predicate);
        int Count(Expression<Func<TCollection, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TCollection, bool>> predicate);
        TCollection GetById(ObjectId id);
        Task<TCollection> GetByIdAsync(ObjectId id);
        TCollection GetById(string id);
        Task<TCollection> GetByIdAsync(string id);
        ObjectId Insert(TCollection collection);
        Task<ObjectId> InsertAsync(TCollection collection);
        void InsertMany(IEnumerable<TCollection> collections);
        Task InsertManyAsync(IEnumerable<TCollection> collections);
        void Update(ObjectId id, TCollection collection);
        Task UpdateAsync(ObjectId id, TCollection collection);
        void Delete(ObjectId id);
        Task DeleteAsync(ObjectId id);
        void Delete(string id);
        Task DeleteAsync(string id);
    }
}
