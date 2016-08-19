using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.Common;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace SecretSharing.Test
{
    [TestClass]
    public class ShamirTests
    {
        [TestMethod]
        public void TestDivideSecret()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = "12345678";
            //assign
            var shares = shamir.DivideSecret(k, n,Encoding.UTF8.GetBytes(secret));
            //assert
            Assert.AreEqual(shares.Count, n);
        }
        [Ignore]
        [TestMethod]
        public void TestInitiateSecretWith_Y_P_Array()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = 1234;
            //assign
            var shares = shamir.DivideSecret(k, n,secret);
            var initiatedShares = new List<IShareCollection>();
            //assert
            Assert.AreEqual(shares.Count, n);
            int j = 0;
            foreach (var col in shares)
            {
                j++;
                IShareCollection collection = new ShareCollection();
                for (int i = 0; i < col.GetCount(); i++)
                {
                   
                    Assert.IsNotNull(col.GetShare(i).GetY());
                    Assert.IsNotNull(col.GetShare(i).GetP());

                    ShamirShare myshare = new ShamirShare(col.GetShare(i).GetX(), col.GetShare(i).GetY()
                        , col.GetShare(i).GetP());

                    Assert.AreEqual(col.GetShare(i).ToString(), myshare.ToString());
                    collection.SetShare(i, myshare);
                }
                initiatedShares.Add(collection);
                if (j == k) break;
            }

            var reconsecret = shamir.ReconstructSecret(initiatedShares);
            Assert.AreEqual(secret,reconsecret);
        }
        [TestMethod]
        public void TestDivideSecretWithChunkSize()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = "1234567890";
            var byteSecret = Encoding.UTF8.GetBytes(secret.ToCharArray());
            byte chunkSize = 5;
            double a = 0;
            //assign
            var shares = shamir.DivideSecret(k, n, byteSecret,chunkSize
#if calcPrimeTime
                ,ref a
#endif
                );
            //assert
            Assert.AreEqual(shares.Count, n);
        }
        [TestMethod]
        public void TestReconstructSecretWithChunkSize()
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var n = 10;
            var k = 3;
            var secret = "1234567890";
            var byteSecret = Encoding.UTF8.GetBytes(secret.ToCharArray());
            byte chunkSize = 5;
            //assign
            double a= 0;
            var shares = shamir.DivideSecret(k, n, byteSecret, chunkSize
#if calcPrimeTime           
                ,ref a
#endif
                );
            //if the secret array is not dividable to the chunk we have to truncate null values
            var reconSecret = Encoding.UTF8.GetString( shamir.ReconstructSecret(shares, chunkSize).Where(ch=>ch !='\0').ToArray());
            //assert
            Assert.AreEqual(shares.Count, n);
            Assert.AreEqual(secret, reconSecret);
        }




        [TestMethod]
        public void TestReconstruct_NoChunk()
        {
            //arrange
            var shamir = new SecretSharingCore.Algorithms.Shamir(); ;
            var n = 10;
            var k = 3;
            var secret = "32456";
            //assign
            var shares = shamir.DivideSecret(k, n,Encoding.UTF8.GetBytes(secret));

            var reconSecret =Encoding.UTF8.GetString(  shamir.ReconstructSecret(shares));
            //assert
            //Assert.AreEqual(k, kPortionOfShares.Count);
            //Assert.AreEqual(shares.GetCount(), n);
            Assert.AreEqual(secret, reconSecret);
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
            ////assert
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
            var secret = "1234567812345678123456781234567812345678123456781234567812345678";
            //assign
            var shares = shamir.DivideStringSecret(k, n, secret,(byte) 32);
            //assert
            Assert.AreEqual(shares.Count, n);
        }
        [TestMethod]
        public void TestStringReconstructSecret()
        {
                SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
                var n = 5;
                var k = 1;
                var secret = "12345678123456781234567812345678";//12345678123456781234567812345678";
                //assign
                var shares = shamir.DivideStringSecret(k, n, secret, (byte)16);
                var kShares = shares.GetRange(0, k);
                var reconsecret = shamir.ReconstructStringSecret(kShares, (byte)16);
                //assert
                Assert.AreEqual(shares.Count, n);
                Assert.AreEqual(secret, reconsecret);
        }
    }
}
