using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.Common;
using SecretSharing.Benchmark;
using System.Collections.Generic;

namespace SecretSharing.Test
{
    [TestClass]
    public class TestXMLSerializer
    {
        [TestMethod]
        public void TestSerializeList()
        {
            var item = 123;
            var path = "a.xml";
            var lista = new List<int>();
            lista.Add(item);

            var serializer = new XMLSerializer<List<int>>();
            serializer.Serialize(lista, path);

            var deslista = serializer.Deserialize(path);

            Assert.IsTrue(deslista.Contains(item));
        }

    
    }
}
