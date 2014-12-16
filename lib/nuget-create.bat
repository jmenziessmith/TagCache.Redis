nuget pack ../src/TagCache.Redis/TagCache.Redis.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget

nuget pack ../src/TagCache.Redis.Json.Net/TagCache.Redis.Json.Net.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget/TagCache.Redis.Json.Net

nuget pack ../src/TagCache.Redis.ProtoBuf/TagCache.Redis.ProtoBuf.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget/TagCache.Redis.ProtoBuf

nuget pack ../src/TagCache.Redis.FastJson/TagCache.Redis.FastJson.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget/TagCache.Redis.FastJson

