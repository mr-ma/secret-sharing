using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretSharing.Benchmark
{
    class Program
    {


        static void HandleReports(PersistanceReport report)
        {
            //group by key and handle for each key following outputs

            int[] PerformanceK = new int[] { 5, 20, 50 };
            var filteredreconrep = report.Reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var filteredgenrep = report.Reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            PDFGenerator pdfreport = new PDFGenerator();

            List<IEnumerable<SecretSharingBenchmarkReport>>[] generationPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];
            List<IEnumerable<SecretSharingBenchmarkReport>>[] reconstructionPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];

            List<string>[] genTitles = new List<string>[PerformanceK.Length];
            List<string>[] reconTitles = new List<string>[PerformanceK.Length];
            for (int i = 1; i <= report.KeySize; i *= 2)
            {
                var chunk = i;
                var queryreconrep = filteredreconrep.Where(po => po.chunkSize == chunk);
                var querygenrep = filteredgenrep.Where(po => po.chunkSize == chunk);


                for (int kiterate = 0; kiterate < PerformanceK.Length; kiterate++)
                {
                    if (generationPerformanceCurves[kiterate] == null) generationPerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    if (genTitles[kiterate] == null) genTitles[kiterate] = new List<string>();
                    var wantedK = PerformanceK[kiterate];

                    var performanceCurveReport = querygenrep.Where(po => po.k == wantedK);
                    if (performanceCurveReport.Count() > 0)
                    {
                        generationPerformanceCurves[kiterate].Add(performanceCurveReport);
                        genTitles[kiterate].Add(string.Format("chunk={0}bit", chunk * 8));
                    }

                    if (reconstructionPerformanceCurves[kiterate] == null) reconstructionPerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    if (reconTitles[kiterate] == null) reconTitles[kiterate] = new List<string>();
                    performanceCurveReport = queryreconrep.Where(po => po.k == wantedK);
                    if (performanceCurveReport.Count() > 0)
                    {
                        reconstructionPerformanceCurves[kiterate].Add(performanceCurveReport);
                        reconTitles[kiterate].Add(string.Format("chunk={0}bit", chunk * 8));
                    }
                }


                pdfreport.GenBenchmarkDoc(string.Format("{0}bit_n{1}_k{2}_recon_chunk{3}bit.pdf", report.KeySize, report.N, report.K, chunk * 8), queryreconrep);
                pdfreport.GenBenchmarkDoc(string.Format("{0}bit_n{1}_k{2}_gen_chunk{3}bit.pdf", report.KeySize, report.N, report.K, chunk * 8), querygenrep);
            }

            for (int l = 0; l < PerformanceK.Length; l++)
            {
                PerormanceAnalysisForm perfViewer = new PerormanceAnalysisForm(generationPerformanceCurves[l], genTitles[l]
                , string.Format("Generate no prime KeySize={0}  K={1} ", report.KeySize, PerformanceK[l])
                , true, false, string.Format("perf_NoPrime_gen_Key{0}bits_K{1}.pdf", report.KeySize, PerformanceK[l]));

                perfViewer = new PerormanceAnalysisForm(generationPerformanceCurves[l], genTitles[l]
              , string.Format("Generate KeySize={0}  K={1}", report.KeySize, PerformanceK[l])
              , false, true, string.Format("perf_Prime_gen_Key{0}bits_K{1}.pdf", report.KeySize, PerformanceK[l]));

                perfViewer = new PerormanceAnalysisForm(generationPerformanceCurves[l], genTitles[l]
      , string.Format("Generate timeKeySize={0}  K={1}", report.KeySize, PerformanceK[l])
      , true, true, string.Format("perf_PrimeAndNoPrime_gen_Key{0}bits_K{1}.pdf", report.KeySize, PerformanceK[l]));
                //perfViewer.ShowDialog();

                perfViewer = new PerormanceAnalysisForm(reconstructionPerformanceCurves[l], reconTitles[l]
               , string.Format("Reconstruct KeySize={0}  K={1}", report.KeySize, PerformanceK[l])
               , false, true, string.Format("perf_recon_Key{0}bits_K{1}.pdf", report.KeySize, PerformanceK[l]));
                // perfViewer.ShowDialog();
            }
        }

        static string BenchmarkToPersistenceStorage()
        {
            ///define the chunk sizes here for the experiment
            int[] Chunks = new int[]{
            1
            ,2,
            4,
            8
            ,16
            ,32
            ,64
            ,128
            ,256
            };

          

            int MaxN = 100;
            int MaxK = 60;
            int step = 5;
            int iterate = 100;
            ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
            //define the key size to experiment here
            //String key = benchmark.key256bit;

            var reports = benchmark.BenchmarkAllKeysWithChunkSize(Chunks, MaxN, MaxK, step, SecretSharingBenchmarkReport.OperationType.ShareGeneration
                 |SecretSharingBenchmarkReport.OperationType.SecretReconstruction
               , iterate);
            var serializer = new XMLSerializer<PersistanceReport>();
            PersistanceReport report = new PersistanceReport();
            report.Reports = reports.ToList();
            report.N = MaxN;
            report.K = MaxK;
            report.Iterate = iterate;
            report.Step = step;
            //report.KeySize = key.Length * 8;

            string filePath = string.Format("SerializedReports-n{0}-k{1}-iterate{2}.xml"
                , /*key.Length * 8*/ MaxN, MaxK, iterate);
            serializer.Serialize(report, filePath);
            return filePath;
            
        }
       // [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 0) {printHelp(); return ;}
            string op = args[0].Trim().ToLower();
            var filePath ="";

            if (op == "benchmark")
            {
                filePath = BenchmarkToPersistenceStorage();
                Console.WriteLine(string.Format("Benchmarking is done!\nResults are dumped to {0}", filePath));
            }
            if (op == "report")
            {
                if (args.Length == 1 || string.IsNullOrEmpty( args[1]) || !File.Exists(args[1])) {
                    printHelp();
                    return ;
                }
                filePath = args[1];

                var ser = new XMLSerializer<PersistanceReport>();
                //var filePath = "SerializedReports-n100-k60-iterate100.xml";
                var report = ser.Deserialize(filePath);
                var rep = from r in report.Reports
                          group r by r.keyLength into g
                          select g;
                foreach (var item in rep)
                {
                    PersistanceReport grep = new PersistanceReport();
                    grep.Reports = item.ToList();
                    grep.KeySize = item.Key;
                    grep.N = report.N;
                    grep.K = report.K;
                    grep.Step = report.Step;
                    grep.Iterate = report.Iterate;

                    HandleReports(grep);
                }
                Console.WriteLine("Report files are generated in current path... Old files has been replaced.");
            }

            Console.ReadLine();
            //Application.Run();
        }

        static void printHelp()
        {
            Console.WriteLine("Instruction to use the benchmarker:\napp benchmark: to run benchmark tests\napp report filepath: to generate pdf & report files\nPress any keys to exit..");
            Console.ReadLine();
        }
    }
}
