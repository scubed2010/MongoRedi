# MongoRedi
MongoRedi is wrapper around the MongoDB driver that exposes the key CRUD operations that are needed to interact with an instance of MongDB.  Additionally, collections can be annotated to allow for all data to be retrieved from Redis rather than going all the way to MongoDB.

## Use Cases
MongoRedi is implemented using .NET Standard, so you can easily integrate it with the .NET Framework or .NET Core.

MongoDB is an excellent database for development and performance when accessing data for your application.  However, as you move toward the cloud, it can become expensive to run instances of MongoDB that require lots of memory.  Leveraging the power of Redis can take some of the burden off of MongoDB, which will lead to even better performance and cost savings.

## Implementation
Install the NuGet package for your application.  If you have a multi-tiered architecture, then you’ll want to make the installation in your data access layer.

https://www.nuget.org/packages/MongoRedi/

### .NET Core
Add the following properties in your appsettings.json file:

```
{
  "MongoRedi": {
    "MongoDBConnectionString": "mongodb://localhost:27017",
    "MongoDBDatabase": "MongoRediTest",
    "RedisConnectionString": "localhost,connectTimeout=1000",
    "RedisDatabase": 0,
    "Cache": true
  }
}
```

The following settings are for a local development environment.  However, as you move toward the cloud you can easily swap your connection strings.

* MongoDBConnectionString - Connection string to MongoDB
* MongoDBDatabase - MongoDB database name
* RedisConnectionString - Connection string to Redis
* RedisDatabase - Redis database number (0 - 15)
* Cache - Globally turn cacheing of data in Redis on or off

Create a simple POCO that you would like to represent the documents for a collection:

```
[CollectionName("students")]
[Cache]
public class Student : BaseCollection
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}
```

* CollectionName **(REQUIRED)**
  * This attribute sets the name of the collection as it will be stored in MongoDB
* Cache (OPTIONAL)
  * This attribute determines if this collection will be stored in Redis
* BaseCollection **(REQUIRED)**
  * This base class contains the \_id of each document, which is required for MondoDB.

In the Sample.Core project you will see how you can inject each collection as a Singleton using the built-in dependency injection functionality:

```
// StartUp.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IMongoDBRepository<Student>, MongoDBRepository<Student>>();
}
```

Or you can instantiate the instance directly:

```
IMongoDBRepository<Student> _studentRepository = new MongoDBRepository<Student>();
```

### .NET Framework
Setup for the .NET Framework is the same except for the following:

Add the following properties to your web.config file:

```
<appSettings>
  <add key="MongoRedi_MongoDBConnectionString" value="mongodb://localhost:27017" />
  <add key="MongoRedi_MongoDBDatabase" value="MongoRediSample" />
  <add key="MongoRedi_RedisConnectionString" value="localhost,connectTimeout=1000" />
  <add key="MongoRedi_RedisDatabase" value="12" />
  <add key="MongoRedi_Cache" value="true" />
</appSettings>
  ```

As for dependency injection, you can use a 3rd Party container, or instantiate directly.
