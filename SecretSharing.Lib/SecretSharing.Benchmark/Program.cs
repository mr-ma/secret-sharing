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
            ///define the chunk sizes here for the experiment
            byte[] Chunks = new Byte[]{
            1
            ,2,
            4,
            8
            ,16
            ,32
            //,64
            //,128
            };

            int[] PerformanceK = new int[] {5,20,50 };

            int MaxN = 100;
            int MaxK = 50;
            int step = 5;
            int iterate = 100;
            ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
            //define the key size to experiment here
            String key = benchmark.key256bit;

            var reports = benchmark.BenchmarkMeWithChunkSize(Chunks, MaxN, MaxK, step, SecretSharingBenchmarkReport.OperationType.ShareGeneration
                 |SecretSharingBenchmarkReport.OperationType.SecretReconstruction
               , key, iterate);

             var filteredreconrep = reports.Where(po => po.Operation== SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var filteredgenrep = reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            PDFGenerator pdfreport = new PDFGenerator();

            List<IEnumerable<SecretSharingBenchmarkReport>>[] generationPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];
            List<IEnumerable<SecretSharingBenchmarkReport>>[] reconstructionPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];
            
            List<string>[] genTitles = new List<string>[PerformanceK.Length];
            List<string>[] reconTitles = new List<string>[PerformanceK.Length];
            for (int i = 1; i <= key.Length * 8; i *= 2)
            {
                var chunk = i;
                 var queryreconrep = filteredreconrep.Where(po => po.chunkSize == chunk);
                var querygenrep = filteredgenrep.Where(po => po.chunkSize == chunk);


                for (int kiterate = 0; kiterate < PerformanceK.Length; kiterate++)
                {
                    if (generationPerformanceCurves[kiterate] == null) generationPerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    if (genTitles[kiterate] == null) genTitles[kiterate] = new List<string>();
                    var wantedK =  PerformanceK[kiterate];

                    var performanceCurveReport = querygenrep.Where(po => po.k == wantedK);
                    if (performanceCurveReport.Count() > 0)
                    {
                        generationPerformanceCurves[kiterate].Add(performanceCurveReport);
                        genTitles[kiterate].Add(string.Format("chunk={0}bit", chunk*8));
                    }

                    if (reconstructionPerformanceCurves[kiterate] == null) reconstructionPerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    if (reconTitles[kiterate] == null) reconTitles[kiterate] = new List<string>();
                    performanceCurveReport = queryreconrep.Where(po => po.k == wantedK);
                    if (performanceCurveReport.Count() > 0)
                    {
                        reconstructionPerformanceCurves[kiterate].Add(performanceCurveReport);
                        reconTitles[kiterate].Add(string.Format("chunk={0}bit", chunk*8));
                    }
                }

            
                pdfreport.GenBenchmarkDoc(string.Format("{0}bit_n{1}_k{2}_recon_chunk{3}bit.pdf",key.Length*8,MaxN,MaxK, chunk*8), queryreconrep);
                pdfreport.GenBenchmarkDoc(string.Format("{0}bit_n{1}_k{2}_gen_chunk{3}bit.pdf", key.Length * 8, MaxN, MaxK, chunk * 8), querygenrep);
            }

            for (int l = 0; l < PerformanceK.Length; l++)
            {
                PerormanceAnalysisForm perfViewer = new PerormanceAnalysisForm(generationPerformanceCurves[l], genTitles[l]
                , string.Format("Generate KeySize={0}  K={1}", key.Length * 8, PerformanceK[l])
                ,true, string.Format("perf_gen_Key{0}bits_K{1}.pdf",key.Length * 8, PerformanceK[l]));
                //perfViewer.ShowDialog();

                perfViewer = new PerormanceAnalysisForm(reconstructionPerformanceCurves[l], reconTitles[l]
               , string.Format("Reconstruct KeySize={0}  K={1}", key.Length * 8, PerformanceK[l])
               ,false, string.Format("perf_recon_Key{0}bits_K{1}.pdf",key.Length * 8, PerformanceK[l]));
               // perfViewer.ShowDialog();
            }
        }
       // [STAThread]
        static void Main(string[] args)
        {
            Benchmark256bitKeyWithPDFs();
            Console.ReadLine();
            //Application.Run();
        }
    }
}
