using MongoRedi.Attributes;

namespace MongoRedi.Tests.MongoRediModels
{
    [CollectionName("wizards")]
    [CacheCollection]
    [CacheDocument]
    public class Wizard : User
    {
    }
}
