using MongoRedi.Attributes;
using MongoRedi.Models;

namespace Sample.Framework.Models
{
    [CollectionName("students")]
    [CacheCollection]
    public class Student : BaseCollection
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}