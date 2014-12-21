using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretSharing.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Byte chunkSize = 8;
            int MaxN = 100;
            int MaxK = 100;
            int step = 5;


       

            ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
            var reports = benchmark.BenchmarkMeWithChunkSize(chunkSize, 50, 50, 5,SecretSharingBenchmarkReport.OperationType.ShareGeneration,benchmark.key256bit, 10);
            var floatchunkreports = benchmark.BenchmarkMeWithChunkFloatSize(50, 50, 5, SecretSharingBenchmarkReport.OperationType.ShareGeneration, benchmark.key256bit, 10);
            
            
            var filteredrep = reports.Where(po => po.chunkSize == 8);

            PDFGenerator pdfreport = new PDFGenerator();
            pdfreport.GenBenchmarkDoc("256bit_n50_k50_chunk64bit.pdf", filteredrep);

            filteredrep = reports.Where(po => po.chunkSize == 4);
            pdfreport.GenBenchmarkDoc("256bit_n50_k50_chunk32bit.pdf", filteredrep);

            filteredrep = reports.Where(po => po.chunkSize == 1);
            pdfreport.GenBenchmarkDoc("256bit_n50_k50_chunk8bit.pdf", filteredrep);

            pdfreport.GenBenchmarkDoc("256bit_n50_k50_floatchunk.pdf", floatchunkreports);
            //benchmark.BenchmarkMeWithChunkSize(chunkSize, MaxN, MaxK, step, "AllResults100N_100K", 10);


            /*var re = benchmark.BenchmarkMeWithChunkSize(chunkSize,MaxN,MaxK,step);
            var k5 = re.Where(po => po.k == 5 && po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            var k10 = re.Where(po => po.k == 10 && po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            var k15 = re.Where(po => po.k == 15 && po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            var k20 = re.Where(po => po.k == 20 && po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            
            var k5c = re.Where(po => po.k == 5 && po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var k10c = re.Where(po => po.k == 10 && po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var k15c = re.Where(po => po.k == 200 && po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var k20c = re.Where(po => po.k == 400 && po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);


            var titles = new List<string>() { "k=5", "k=10", "k=15", "k=20", "kc=5", "kc=10", "kc=200", "kc=400" };

            System.IO.File.WriteAllLines("k5_benchmarkresults.txt", k15c.Select(po=>po.ToString()));
            System.IO.File.WriteAllLines("k5c_benchmarkresults.txt", k20c.Select(po => po.ToString()));
            System.IO.File.WriteAllLines("AllResults.txt", re.Select(po => po.ToString()));
            var reports = new List<IEnumerable<SecretSharingBenchmarkReport>>() { k5,k10,k15,k20,k5c,k10c,k15c,k20c};
            Application.Run(new PerormanceAnalysisForm(reports,titles));*/


        }
    }
}
