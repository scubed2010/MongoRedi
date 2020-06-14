using System;

namespace MongoRedi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CacheDocumentAttribute : Attribute
    {
    }
}
