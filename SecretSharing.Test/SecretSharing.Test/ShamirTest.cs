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
            //arrange
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 10;
            var k = 3;
            var secret = 1234;
            //assign
            var shares =  shamir.DivideSecret(secret, k, n);
            //assert
            Assert.AreEqual(shares.Count, n);
        }

        [TestMethod]
        public void ReconstructSecretTest()
        {
            //arrange
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
        public void ReconstructTextSecretTest()
        {
            //arrange
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 3;
            var k = 2;
            var secret = "ABC";
            //assign
            var shares = shamir.DivideSecret(secret, k, n);

            var kPortionOfShares = shares.GetRange(0, k);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret);

        }
        [TestMethod]
        public void FailToReconstructSecretTest()
        {
            //arrange
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
        [TestMethod]
        public void Reconstruct256bitSecretTest()
        {
            //arrange
            var randomAlgorithm = new SimpleRandom();
            var shamir = new ShamirSecretSharing(randomAlgorithm);
            var n = 10;
            var k = 3;
            var secrets = randomAlgorithm.GetRandomArray(32,0,255);
            for (int i = 0; i < 32; i++)
            {
                //assign
                var shares = shamir.DivideSecret(secrets[i], k, n);

                var kPortionOfShares = shares.GetRange(0, k);

                var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
                //assert
                Assert.AreEqual(k, kPortionOfShares.Count);
                Assert.AreEqual(shares.Count, n);
                Assert.AreEqual(secrets[i], reconSecret.Value);
            }

        }

    }
}
