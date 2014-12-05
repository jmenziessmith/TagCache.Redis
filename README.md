TagCache.Redis
==============

.Net Redis Cache with support for tagging

##Features
 * Simple key value caching
 * Cache item expiry
 * Store cache items with tags
 * Retrieve cache items by tag
 * Remove all items by tag
 * Multiple Serialization options 
 
### Roadmap
* Expiry notification events
* Multiplexed caching (sync with in memory cache)
* Upgrade from booksleeve to StackExchange.Redis 

## Usage

### Configuration
```c#
var config = new CacheConfiguration()
	{
		RootNameSpace = "_RedisCache",
		RedisClientConfiguration = new RedisClientConfiguration()
		{
			Host = "localhost",
			DbNo = 0,
			TimeoutMilliseconds = 500
		}
	};
var cache = new RedisCacheProvider(config);
```

### No Configuration
```c#
var cache = new RedisCacheProvider(); // will default to localhost
```

### Adding Items
#### Add an item without tags
```c#
string key = "TagCacheTests:Add";
String value = "Hello World!";
DateTime expires = new DateTime(2099, 12, 11); 
cache.Set(key, value, expires);
```

#### Add an item with tags
```c#
string key = "TagCacheTests:Add";
String value = "Hello World!";
DateTime expires = new DateTime(2099, 12, 11);
var tags = new List<string> { "tag1", "tag2", "tag3" };
cache.Set(key, value, expires, tags);
```

### Get Items
#### Get an item by key
```c#
var test = cache.Get<String>("mykey");
```

#### Get items by tag
```c#
var results = cache.GetByTag<String>("tag2");
```

### Remove items
#### Remove item by key
```c#
cache.Remove("key1");
```

```c#
cache.Remove(new string[] { "key2", "key3" });
```

#### Remove items by tag
```c#
cache.RemoveByTag("tag1");
```






