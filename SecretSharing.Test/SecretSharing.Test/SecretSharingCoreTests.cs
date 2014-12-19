using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.Common;
using SecretSharing.Lib.Common;
using System.Text;
namespace SecretSharing.Test
{
    [TestClass]
    public class SecretSharingCoreTests
    {
        [TestMethod]
        public void TestDivideSecret()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = 1234;
            //assign
            var shares = shamir.DivideSecret(k,n,secret);
            //assert
            Assert.AreEqual(shares.Count, n);
        }

        [TestMethod]
        public void TestDivideSecretWithChunkSize()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = "1234567890";
            var byteSecret = Encoding.UTF8.GetBytes(secret.ToCharArray());
            byte chunkSize = 8;
            //assign
            var shares = shamir.DivideSecret(k, n, byteSecret,chunkSize);
            //assert
            Assert.AreEqual(shares.Count, n);
        }




        public void TestDivideSecret(int n,int k,string Secret)
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            //assign
            var shares = shamir.DivideSecret(k, n, Secret);
          
        }
        public void TestDivideSecret(int n)
        {
            var k = 5;
            var Secret = "Test";
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            //assign
            var shares = shamir.DivideSecret(k, n, Secret);

        }
        [TestMethod]
        public void ReconstructSecretTest()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); ;
            var n = 10;
            var k = 3;
            var secret = 2146985113;
            //assign
            var shares = shamir.DivideSecret(k, n, secret);
            //  Assert.IsFalse(shamir.GetPrime() < 0);

            var kPortionOfShares = shares.GetRange(0, k);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            if (secret != reconSecret)
            {
                Console.WriteLine(shamir.GetPrime());
            }

            //assert
            Assert.AreEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret);
        }

        [TestMethod]
        public void ReconstructSecretTestRandomAttempt()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); ;
            var n = 10;
            var k = 3;
            var counter = 1000;
            Random r = new Random();
            while (counter>0)
            {
                counter--;
                var secret = r.Next();
                //assign
                var shares = shamir.DivideSecret(k, n, secret);
                var kPortionOfShares = shares.GetRange(0, k);

                var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
                //assert
                Assert.AreEqual(k, kPortionOfShares.Count);
                Assert.AreEqual(shares.Count, n);
                Assert.AreEqual(secret, reconSecret);

            }
        }
        [TestMethod]
        public void FailReconstructSecretTest()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); ;
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


        [TestMethod]
        public void TestStringDivideSecret()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = "1234";
            //assign
            var shares = shamir.DivideSecret(k, n, secret);
            //assert
            Assert.AreEqual(shares.Count, n);
        }
        [TestMethod]
        public void ReconstructStringSecretTest()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); ;
            var n = 10;
            var k = 3;
            var secret = "2345";
            //assign
            var shares = shamir.DivideSecret(k, n, secret);

            var kPortionOfShares = shares.GetRange(0, k);

            var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
            //assert
            Assert.AreEqual(k, kPortionOfShares.Count);
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret);
        }
        [TestMethod]
        public void FailReconstructStringSecretTest()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); 
            var n = 10;
            var k = 3;
            var secret = "2345";
            //assign
            var shares = shamir.DivideSecret(k, n, secret);

            var kPortionOfShares = shares.GetRange(0, k - 1);

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
            var shamir = new SecretSharingCore.Algorithms.Shamir(); 
            var n = 10;
            var k = 3;
            var secrets = randomAlgorithm.GetRandomArray(32, 0, 255);
            for (int i = 0; i < 32; i++)
            {
                //assign
                var shares = shamir.DivideSecret(k,n,secrets[i]);

                var kPortionOfShares = shares.GetRange(0, k);

                var reconSecret = shamir.ReconstructSecret(kPortionOfShares);
                //assert
                Assert.AreEqual(k, kPortionOfShares.Count);
                Assert.AreEqual(shares.Count, n);
                Assert.AreEqual(secrets[i], reconSecret);
            }

        }
    }
}
