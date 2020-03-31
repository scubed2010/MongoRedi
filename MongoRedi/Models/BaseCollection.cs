using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoRedi.Models
{
    public abstract class BaseCollection
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
