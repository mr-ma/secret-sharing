using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.Algorithms.GeneralizedAccessStructure;
using SecretSharing.OptimalThreshold.Models;
using SecretSharing.OptimalThreshold;
using System.Text;

namespace SecretSharing.Test.Core
{
    [TestClass]
    public class BenalohLeichterTest
    {
        [TestMethod]
        public void TestDivideSecret()
        {
            //assign
            var secret = "12345678";
            var secretbytes = Encoding.UTF8.GetBytes(secret.ToCharArray());
            var benaloh = new BenalohLeichter();
            var access = new AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4");
            var  tryIntersect  = true;

            //arrange
            var optimisedAccess = ThresholdHelper.OptimiseAccessStructure(access, tryIntersect);
            var shares = benaloh.DivideSecret(secretbytes,optimisedAccess);

            //assert
            Assert.IsNotNull(shares);
            Assert.IsTrue(shares.Count > 0);
        }
        [TestMethod]
        public void TestReconstructSecret()
        {
            //assign
            var secret = "12345678";
            var secretbytes = Encoding.UTF8.GetBytes(secret.ToCharArray());
            var benaloh = new BenalohLeichter();
            var access = new AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4");
            var tryIntersect = true;

            //arrange
            var optimisedAccess = ThresholdHelper.OptimiseAccessStructure(access, tryIntersect);
            var shares = benaloh.DivideSecret(secretbytes, access);

            foreach (var item in shares)
            {
                var reconSecret = Encoding.UTF8.GetString( benaloh.ReconstructSecret(item));
                
                Assert.AreEqual(secret, reconSecret,"secret and reconstructed secret are not the same");
            }
            
 
            //assert
            Assert.IsNotNull(shares);
            Assert.IsTrue(shares.Count > 0);
           
        }
    }
}
