using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Serialization;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture(Category = "Proto-buf tests,serialization")]
    public class ProtoBufSerializationProviderTests : SerializationProviderTestsBase
    { 
        protected override ISerializationProvider GetSerializer()
        {
            return new ProtoBufSerializationProvider();
        }
    }
}