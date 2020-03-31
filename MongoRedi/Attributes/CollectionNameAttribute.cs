using System;

namespace MongoRedi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CollectionNameAttribute : Attribute
    {
        public string CollectionName { get; private set; }


        public CollectionNameAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
