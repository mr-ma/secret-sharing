using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharing.Lib.Common;
namespace SecretSharing.Test
{
    [TestClass]
    public class SimpleRandomTest
    {
        [TestMethod]
        public void TestGetRandomArray()
        {
            //Arrange
            var rand = new SimpleRandom();
            var min = 1;
            var max = 900;
            var length = 10;
            int[] randomValues;
            //Assign
            randomValues = rand.GetRandomArray(length, min, max);
            //Assert
            foreach (var value in randomValues)
            {
                Assert.IsTrue(value >= min);
                Assert.IsTrue(value <= max);
            }
        }


        [TestMethod]
        public void TestGetRandomPrime()
        {
            //Arrange
            var rand = new SimpleRandom();
            var p = 1;
            //Assign
            p = rand.GetRandomPrimeNumber();
            //Assert
            //just a dummy test
            Assert.IsFalse(p % 5 == 0);
        }
    }
}
