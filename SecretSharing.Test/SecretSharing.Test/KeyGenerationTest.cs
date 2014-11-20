using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Security.Cryptography;
using SecretSharing.FiniteFieldArithmetic;

namespace SecretSharing.Test
{
    [TestClass]
    public class KeyGenerationTest
    {
        [TestMethod]
        public void Test256Key()
        {
            var key = "4AC5F3173FC753FDC21EEFBAFB793XXX";
            var bytes = Encoding.UTF8.GetBytes(key);
            Assert.AreEqual(256, bytes.Length * 8);
        }

        [TestMethod]
        public void TestPrimeGeneration3()
        {
            var primes = MathTools.GeneratePrime3(10000000);
            Assert.AreEqual(664579, primes.Count);
        }

        [TestMethod]
        public void TestPrimeGeneration4()
        {
            var primes = MathTools.GeneratePrime4(10000000);
            Assert.AreEqual(664579, primes.Count);
        }
        [TestMethod]
        public void TestPrimeGeneration5()
        {
            var primes = MathTools.GeneratePrime5(100000000);
            Assert.AreEqual(664579, primes.Count);
        }
    }
}
