using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharing.OptimalThreshold.Models;
using SecretSharing.OptimalThreshold;
using System.Collections.Generic;

namespace SecretSharing.Test.OptimalThreshold
{
    [TestClass]
    public class ThresholdHelperTest
    {
        [TestMethod]
        public void IsThresholdShareShorterTestPass()
        {
            //assign 
            var nothresholdAccess = "p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7";
            var tryIntersect = true;
            //arrange
            AccessStructure a = new AccessStructure(nothresholdAccess);
            var thresholds = new List<ThresholdSubset>();
            var remaining = new List<QualifiedSubset>();
            ThresholdHelper.DetectThresholds(a,tryIntersect, out thresholds, out remaining);
            bool isEfficicent = ThresholdHelper.IsThresholdShareShorter(a, thresholds, remaining);

            //assert
            Assert.IsTrue(isEfficicent);
        }
        [TestMethod]
        public void IsThresholdShareShorterTestFail()
        {
            //assign 
            var nothresholdAccess = "P1^P2,P2^P3,P3^p4,p4^p5,p5^p6,p6^p7,p7^p8,p8^p1";
            var tryIntersect = true;
            //arrange
            AccessStructure a = new AccessStructure(nothresholdAccess);
            var thresholds = new List<ThresholdSubset>();
            var remaining = new List<QualifiedSubset>();
            ThresholdHelper.DetectThresholds(a,tryIntersect, out thresholds,out remaining);
            bool isEfficicent = ThresholdHelper.IsThresholdShareShorter(a,thresholds,remaining);

            //assert
            Assert.IsFalse(isEfficicent);
        }
        

    }
}
