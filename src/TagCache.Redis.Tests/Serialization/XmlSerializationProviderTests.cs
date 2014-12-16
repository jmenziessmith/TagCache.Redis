using System.Diagnostics;
using NUnit.Framework; 
using System;
using System.Collections.Generic;
using System.Linq;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Serialization; 

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class XmlSerializationProviderTests : SerializationProviderTestsBase
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new XmlSerializationProvider();
        }
    }
}