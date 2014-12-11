using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SecretSharing.Test
{
    [TestClass]
    public class SecretSharingCoreTests
    {
        [TestMethod]
        public void TestDivideSecret()
        {
            SecretSharing.Shamir.Shamir shamir = new Shamir.Shamir();
            var n = 10;
            var k = 3;
            var secret = 1234;
            //assign
            var shares = shamir.DivideSecret(k,n,secret);
            //assert
            Assert.AreEqual(shares.Count, n);
        }
        [TestMethod]
        public void ReconstructSecretTest()
        {
            //arrange
            var shamir = new Shamir.Shamir(); ;
            var n = 10;
            var k = 3;
            var secret = 2345;
            //assign
            var shares = shamir.DivideSecret(k,n,secret);
            var kPortionOfShares = shares.GetRange(0, k);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret);
        }
        [TestMethod]
        public void FailReconstructSecretTest()
        {
            //arrange
            var shamir = new Shamir.Shamir(); ;
            var n = 10;
            var k = 3;
            var secret = 2345;
            //assign
            var shares = shamir.DivideSecret(k,n,secret);

            var kPortionOfShares = shares.GetRange(0, k-1);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreNotEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreNotEqual(secret, reconSecret);
        }
    }
}
