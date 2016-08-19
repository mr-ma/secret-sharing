using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharing.Benchmark;
using System.Linq;

namespace SecretSharing.Test.Benchmark
{
#if calcPrimeTime
    [TestClass]
    public class BenchmarkTest
    {
        [TestMethod]
        public void TestPrimeTimeMinusTime()
        { 
            //arrange
            var serialisedFile = @"C:\Users\MAhmadvand.BRAINLOOP\Source\Repos\SecretSharing\SecretSharing.Lib\SecretSharing.Benchmark\bin\x64\Debug\SerializedReports-tickMode.xml";

            //assign
            XMLSerializer<PersistanceReport> ser = new XMLSerializer<PersistanceReport>();
            var report = ser.Deserialize(serialisedFile);

            //assert
            foreach (var item in report.Reports)
            {
                if(item.Operation== SecretSharingBenchmarkReport.OperationType.ShareGeneration)
                Assert.IsTrue(item.TotalElapsedMilliseconds - item.primeGenerationTime >= 0);
            }
        }
        [TestMethod]
        public void TestMeanSquareAverage()
        {
            //arrange
            var serialisedFile = @"C:\Users\MAhmadvand.BRAINLOOP\Source\Repos\SecretSharing\SecretSharing.Lib\SecretSharing.Benchmark\bin\x64\Debug\SerializedReports-tickMode.xml";

            //assign
            var ser = new XMLSerializer<PersistanceReport>();
            var report = ser.Deserialize(serialisedFile);
            var rep = from r in report.Reports
                      group r by r.keyLength into g
                      select g;

            //assert
            foreach (var reports in rep)
            {
                var sumPrimeGenTime = reports.Select(po =>Math.Pow( po.primeGenerationTime,2.0)).Aggregate((current, next) => current + next);
                 Assert.IsFalse(double.IsInfinity(sumPrimeGenTime));
                var rmsPrimeTime = Math.Sqrt(1.0 / (double) reports.Count() * (sumPrimeGenTime));
                Assert.IsFalse(double.IsNaN(rmsPrimeTime));
                Console.WriteLine(rmsPrimeTime);
            }
        }
    }
#endif
}
