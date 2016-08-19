using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.OleDb;
using SecretSharing.Benchmark;
using System.Text;
using SecretSharingCore;
using System.Diagnostics;

namespace SecretSharing.Test.Common
{
    [TestClass]
    public class IndocryptResultParser
    {[Ignore]
        [TestMethod]
        public void TestGetResultsFromExcell()
        {
            var operation = SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction;
            var filename = "256recon";
            var keylength = 256;


            var benchmarks = new LinqToExcel.ExcelQueryFactory(filename+".xlsx");
           
            var results = new List<SecretSharingBenchmarkReport>();
            foreach (var row in benchmarks.Worksheet(0))
            {
                var n = row[0].Cast<int>();
                for (int k = 0; k <= 10&&k*5<=n; k++)
                {
                    var report = new SecretSharingBenchmarkReport();
                    report.keyLength = keylength;
                    report.Operation = operation;
                    report.n = n;
                    report.k = k == 0 ? 1 : k * 5;
                    report.TotalElapsedMilliseconds =Convert.ToDouble( 
                        row[k+1].Cast<string>().Split('\n')[1]);
                    results.Add(report);
                }
            }

            var serializer = new XMLSerializer<PersistanceReport>();
            PersistanceReport pr = new PersistanceReport();
            pr.Reports = results;
            serializer.Serialize(pr, "results\\"+filename + ".xml");
        }
       
        [Ignore]
        [TestMethod]
        public void TestSerializeIndocryptResults()
        {
            var filename = "indoresults";
            var serializer = new XMLSerializer<PersistanceReport>();
            var report= serializer.Deserialize("results\\" + filename + ".xml");
        }
    }
}
