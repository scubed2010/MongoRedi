using System;

namespace MongoRedi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CacheCollectionAttribute : Attribute
    {
    }
}
