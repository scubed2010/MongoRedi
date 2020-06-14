using System;

namespace MongoRedi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Obsolete("Use the CacheCollectionAttribute. To be removed in Version 2.0.")]
    public sealed class CacheAttribute : Attribute
    {
    }
}
