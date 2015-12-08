nuget pack ../src/TagCache.Redis/TagCache.Redis.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget

nuget pack ../src/TagCache.Redis.Json.Net/TagCache.Redis.Json.Net.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget/TagCache.Redis.Json.Net

nuget pack ../src/TagCache.Redis.ProtoBuf/TagCache.Redis.ProtoBuf.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget/TagCache.Redis.ProtoBuf

nuget pack ../src/TagCache.Redis.FastJson/TagCache.Redis.FastJson.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget/TagCache.Redis.FastJson

nuget pack ../src/TagCache.Redis.Migrant/TagCache.Redis.Migrant.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget/TagCache.Redis.Migrant

nuget pack ../src/TagCache.Redis.MessagePack/TagCache.Redis.MessagePack.csproj -Build -Symbols -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=nuget" -OutputDirectory nuget/TagCache.Redis.MessagePack

