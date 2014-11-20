using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharing.Lib.Common;
using SecretSharing.Lib.Shamir;

namespace SecretSharing.Test
{
    [TestClass]
    public class ShamirTest
    {
        [TestMethod]
        public void DivideSecretTest()
        {
            //arrang
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 10;
            var k = 3;
            var secret = 2345;
            //assign
            var shares =  shamir.DivideSecret(secret, k, n);
            //assert
            Assert.AreEqual(shares.Count, n);
        }

        [TestMethod]
        public void ReconstructSecretTest()
        {
            //arrang
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 10;
            var k = 3;
            var secret = 2345;
            //assign
            var shares = shamir.DivideSecret(secret, k, n);

            var kPortionOfShares = shares.GetRange(0, k);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret.Value);

        }

        [TestMethod]
        public void FailToReconstructSecretTest()
        {
            //arrang
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 10;
            var k = 3;
            var secret = 2345;
            //assign
    
            var shares = shamir.DivideSecret(secret, k, n);

            //k-1 must not be able to recon the secret
            var kPortionOfShares = shares.GetRange(0, k-1);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreNotEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreNotEqual(secret, reconSecret);

        }
    }
}
