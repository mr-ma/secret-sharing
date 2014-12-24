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



        static void Benchmark256bitKeyWithPDFs()
        {
            Byte chunkSize = 8;
            int MaxN = 50;
            int MaxK = 50;
            int step = 5;
            int iterate = 1;



            ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
            var reports = benchmark.BenchmarkMeWithChunkSize(chunkSize,MaxN, MaxK, step, SecretSharingBenchmarkReport.OperationType.ShareGeneration
                |SecretSharingBenchmarkReport.OperationType.SecretReconstruction, benchmark.key256bit, iterate);
          
           // var filteredreconrep = reports.Where(po => po.Operation== SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var filteredgenrep = reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            PDFGenerator pdfreport = new PDFGenerator();
            for (int i = 1; i < 512; i*=2)
            {
                var chunk = i;
               // var queryreconrep = filteredreconrep.Where(po => po.chunkSize == chunk);
                var querygenrep = filteredgenrep.Where(po => po.chunkSize == chunk);

               // pdfreport.GenBenchmarkDoc(string.Format("256bit_n50_k50_recon_chunk{0}bit.pdf", chunk), queryreconrep);
                pdfreport.GenBenchmarkDoc(string.Format("256bit_n50_k50_gen_chunk{0}bit.pdf", chunk*8), querygenrep);
            }
        }
        static void Main(string[] args)
        {
            Benchmark256bitKeyWithPDFs();
        }
    }
}
