﻿using MongoRedi.Attributes;
using MongoRedi.Models;

namespace MongoRedi.Tests.MongoRediModels
{
    [CollectionName("users")]
    public class User : BaseCollection
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
