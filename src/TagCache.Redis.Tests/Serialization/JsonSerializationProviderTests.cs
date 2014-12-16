using System.Diagnostics;
using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Serialization; 
using System;
using System.Collections.Generic;
using System.Linq;
using TagCache.Redis.Json.Net;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class JsonSerializationProviderTests : SerializationProviderTestsBase
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new JsonSerializationProvider();
        }
    }
}
