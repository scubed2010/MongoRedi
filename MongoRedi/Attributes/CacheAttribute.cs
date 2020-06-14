﻿using System;

namespace MongoRedi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Obsolete("Use the CacheCollectionAttribute moving forward.")]
    public sealed class CacheAttribute : Attribute
    {
    }
}
