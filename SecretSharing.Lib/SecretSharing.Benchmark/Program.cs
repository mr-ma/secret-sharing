using SecretSharing.OptimalThreshold.Models;
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


        static void HandleReports(PersistanceReport report,string output="")
        {
            //group by key and handle for each key following outputs

            int[] PerformanceK = new int[] { 5, 20, 50 };
            int[] PerformanceN = new int[] {50, 100 };

            var filteredreconrep = report.Reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
            var filteredgenrep = report.Reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration);
            var filteredprime = report.Reports.Where(po => po.Operation == SecretSharingBenchmarkReport.OperationType.PrimeGneration);

            PDFGenerator pdfreport = new PDFGenerator();

            List<IEnumerable<SecretSharingBenchmarkReport>>[] generationPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];
            List<IEnumerable<SecretSharingBenchmarkReport>>[] reconstructionPerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceN.Length];
            List<IEnumerable<SecretSharingBenchmarkReport>>[] primePerformanceCurves = new List<IEnumerable<SecretSharingBenchmarkReport>>[PerformanceK.Length];

            List<string>[] genTitles = new List<string>[PerformanceK.Length];
            List<string>[] primeTitles = new List<string>[PerformanceK.Length];
            List<string>[] reconTitles = new List<string>[PerformanceN.Length];
            for (int i = 1; i <= report.KeySize; i *= 2)
            {
                var chunk = i;
                var queryreconrep = filteredreconrep.Where(po => po.chunkSize == chunk);
                var querygenrep = filteredgenrep.Where(po => po.chunkSize == chunk);
                var queryprime = filteredprime.Where(po => po.chunkSize == chunk);

                for (int kiterate = 0; kiterate < PerformanceK.Length; kiterate++)
                {
                    if (generationPerformanceCurves[kiterate] == null)
                    {
                        generationPerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                         
                    }
                    if (primePerformanceCurves[kiterate] == null)
                    {
                        primePerformanceCurves[kiterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    }
                    if (genTitles[kiterate] == null)
                    {
                        genTitles[kiterate] = new List<string>();
                    }
                    if (primeTitles[kiterate] == null)
                    {
                        primeTitles[kiterate] = new List<string>();
                    }
                    var wantedK = PerformanceK[kiterate];

                    var performanceCurveReport = querygenrep.Where(po => po.k == wantedK);
                    if (performanceCurveReport.Count() > 0)
                    {
                        generationPerformanceCurves[kiterate].Add(performanceCurveReport);
                        genTitles[kiterate].Add(string.Format("chunk={0}bit", chunk * 8));
                    }

                    var performancePrimeCurve = queryprime.Where(po => po.k == wantedK);
                    if (performancePrimeCurve.Count() > 0)
                    {
                        primePerformanceCurves[kiterate].Add(performancePrimeCurve);
                        primeTitles[kiterate].Add(string.Format("chunk={0}bit", chunk * 8));
                    }
                }

                for (int niterate = 0; niterate < PerformanceN.Length; niterate++)
                {
                    var wantedN = PerformanceN[niterate];
                    var performanceCurveReport = querygenrep.Where(po => po.n == wantedN);
                    if (reconstructionPerformanceCurves[niterate] == null) reconstructionPerformanceCurves[niterate] = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                    if (reconTitles[niterate] == null) reconTitles[niterate] = new List<string>();
                    performanceCurveReport = queryreconrep.Where(po => po.n == wantedN);
                    if (performanceCurveReport.Count() > 0)
                    {
                        reconstructionPerformanceCurves[niterate].Add(performanceCurveReport);
                        reconTitles[niterate].Add(string.Format("chunk={0}bit", chunk * 8));
                    }
                }

                pdfreport.GenBenchmarkDoc( string.Format("{0}{1}bit_n{2}_k{3}_recon_chunk{4}bit.pdf",output ,report.KeySize, report.N, report.K, chunk * 8), queryreconrep);
                pdfreport.GenBenchmarkDoc(string.Format("{0}{1}bit_n{2}_k{3}_gen_chunk{4}bit.pdf",output, report.KeySize, report.N, report.K, chunk * 8), querygenrep);
            }

            PerormanceAnalysisForm perfViewer = new PerormanceAnalysisForm();
            for (int l = 0; l < PerformanceK.Length; l++)
            {
#if calcPrimeTime
                perfViewer.ExportPerformanceReportsAsPNG_PDF(generationPerformanceCurves[l], genTitles[l]
                , string.Format("Generate no prime KeySize={0}  K={1} ", report.KeySize, PerformanceK[l])
                , true, false,
                string.Format("{0}perf_NoPrime_gen_Key{1}bits_K{2}", output, report.KeySize, PerformanceK[l])
                );

                 perfViewer.ExportPerformanceReportsAsPNG_PDF(generationPerformanceCurves[l], genTitles[l]
      , string.Format("Generate timeKeySize={0}  K={1}", report.KeySize, PerformanceK[l]), true, true,
      string.Format("{0}perf_PrimeAndNoPrime_gen_Key{1}bits_K{2}", output, report.KeySize, PerformanceK[l])
      );
               
#endif
                perfViewer.ExportPerformanceReportsAsPNG_PDF(generationPerformanceCurves[l], genTitles[l]
                             , string.Format("Share Generation KeySize={0}  K={1}", report.KeySize, PerformanceK[l])
                             , false, true, po => po.n, "n", po => (double)po.Average() / (double)TimeSpan.TicksPerMillisecond,"ms",
                             string.Format("{0}perf_Gen_Key{1}bits_K{2}", output, report.KeySize, PerformanceK[l])
                             );
                perfViewer.ExportPerformanceReportsAsPNG_PDF(primePerformanceCurves[l], primeTitles[l]
                             , string.Format("Prime KeySize={0}  K={1}", report.KeySize, PerformanceK[l])
                             , false, true, po => po.n, "n", po => perfViewer.StandardDeviation(po.ToList()),"Standard-D",
                             string.Format("{0}perf_Prime_Key{1}bits_K{2}", output, report.KeySize, PerformanceK[l])
                             );
            }

            for (int l = 0; l < PerformanceN.Length; l++)
            {
                perfViewer.ExportPerformanceReportsAsPNG_PDF(reconstructionPerformanceCurves[l], reconTitles[l]
               , string.Format("Reconstruction KeySize={0}  N={1}", report.KeySize, PerformanceN[l])
               , false, true, po => po.k, "k", po => (double)po.Average() / (double)TimeSpan.TicksPerMillisecond,"ms",
               string.Format("{0}perf_recon_Key{1}bits_N{2}", output, report.KeySize, PerformanceN[l])
               );
            }
            
            
        }


        static void HandleBenalohSchemeReport(BenalohPersistanceReport report, string output)
        {
        }
        static void HandleBenalohShareReport(BenalohPersistanceReport report, string output)
        {
            var countOptimised = 0;
            var sumNumberOfShares = 0;
            var diffEnhancedShares = 0;
            var totalNumberOfShares = 0;
            List<string> lines = new List<string>();
            var enhancedItems = new List<int>();
            foreach (var item in report.Reports)
            {
                var templist = new List<BenalohLeichterBenchmarkReport>();
                templist.Add(item.divideItem);
                templist.Add(item.IntersectedItem);
                templist.Add(item.optimisedItem);

                var min = templist.Min(so => so.NumberOfShares);
                var minitem = templist.Where(po => po.NumberOfShares == min);
                var divitem = item.divideItem;//item.reports.Where(po => po.Operation == OperationType.DivideSecret).First();
                totalNumberOfShares += divitem.NumberOfShares;
                if (minitem.Count() == 3) continue;
                var bestsol = minitem.First();
                if (bestsol.Operation == OperationType.DivideSecretOptimised || bestsol.Operation == OperationType.DivideSecretOptimisedIntersected)
                {
                    enhancedItems.Add(divitem.NumberOfShares - bestsol.NumberOfShares);

                    diffEnhancedShares += divitem.NumberOfShares - bestsol.NumberOfShares;
                    sumNumberOfShares += divitem.NumberOfShares;
                    lines.Add(String.Format("original path = {0} shares ={1}, optimised path ={2} shares ={3}",
                        divitem.Access, divitem.NumberOfShares, bestsol.OptimisedAccess, bestsol.NumberOfShares));
                    countOptimised++;
                }
            }
            lines.Add(string.Format("{0} optimised access out of {1}, Total Shares:{2}, Sum optimisable shares:{3}, Totol Decreased Share:{4}"
                , countOptimised, report.Reports.Count, totalNumberOfShares, sumNumberOfShares, diffEnhancedShares));
            float percentImprovedAccess = (float)(countOptimised * 100.00) / (float)report.Reports.Count();
            float percentDecreaseInImproved = (float)(diffEnhancedShares * 100.00) / (float)sumNumberOfShares;
            float percentDecreaseInAllShares = (float)(diffEnhancedShares * 100.00) / (float)totalNumberOfShares;
            float avgDecreasedShares = (float)diffEnhancedShares / (float)countOptimised;
            //string enhancedelements = enhancedItems.Select(po => po.ToString()).Aggregate((current, next) => current + "," + next);
            float stdDecreasedShares = calculateSTD(enhancedItems, avgDecreasedShares);
            lines.Add(string.Format("Percentage of improved accesses: {0}, Percentage of decreased shares in improvable access shares:{1}" +
                "percentage of improved of all shares:{2}, average of improvement on improvable accesses:{3} , st-d of improvement on improvable accesses:{4} ", percentImprovedAccess,
                percentDecreaseInImproved, percentDecreaseInAllShares, avgDecreasedShares, stdDecreasedShares));

            System.IO.File.WriteAllLines(output, lines);
        }
        static void HandleBenalohReport(BenalohPersistanceReport report, string output,string type)
        {

            switch (type)
            {
                case "scheme":
                    HandleBenalohSchemeReport(report, output);
                    break;
                case "share":
                    HandleBenalohShareReport(report, output);
                    break;
            }
            //var filtered = report.Reports.Where(op=>op.Operation!=OperationType.ReconstructSecret);
            //var grouped = filtered.GroupBy(po=>po.Access).Select(grp=>new {grp.Key , reports = grp.ToList() });
            
        }
        private static float calculateSTD(List<int> items, float avg){
            var sumstd = 0.0;
            for (int i = 0; i < items.Count; i++)
			{
                sumstd+=Math.Pow( ((float) items[i]) - avg,2);
			}
            return Convert.ToSingle(Math.Sqrt(sumstd/(items.Count-1)));
        }
        static void HandleReportsGeneric(PersistanceReport report, string output,PersistanceReport reportcompared)
        {
            //group by key and handle for each key following outputs

            int[] PerformanceK = new int[] { 5, 20, 50 };
            int[] PerformanceN = new int[] { 50, 100 };


            var filtered = report.Reports.GroupBy(rp => new { opr = rp.Operation, keyLength = rp.keyLength })
                .Select(grp => new { KeyLength = grp.Key.keyLength, Operation = grp.Key.opr, 
                    Reports = report.Reports.Where(po => po.Operation == grp.Key.opr && po.keyLength == grp.Key.keyLength) });

            var filteredcompared = reportcompared.Reports.GroupBy(rp => new { opr = rp.Operation, keyLength = rp.keyLength })
                .Select(grp => new
                {
                    KeyLength = grp.Key.keyLength,
                    Operation = grp.Key.opr,
                    Reports = reportcompared.Reports.Where(po => po.Operation == grp.Key.opr && po.keyLength == grp.Key.keyLength)
                });
            PDFGenerator pdfreport = new PDFGenerator();


            foreach (var reportGroup in filtered)
            {
                PerormanceAnalysisForm perfViewer = new PerormanceAnalysisForm();
                var curves = new List<IEnumerable<SecretSharingBenchmarkReport>>();
                //curves.Add(reportGroup.Reports);
                var titles = new List<String>();// { reportGroup.Operation.ToString() };
                Func<SecretSharingBenchmarkReport, long> selector;
                string selectorTitle = "n";
                switch (reportGroup.Operation)
                {

                    case SecretSharingBenchmarkReport.OperationType.SecretReconstruction:

                    case SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction:
                    case SecretSharingBenchmarkReport.OperationType.SpecialSecretReconstruction:
                        selector = delegate(SecretSharingBenchmarkReport po) { return po.k; };
                        selectorTitle = "k";

                        break;

                    case SecretSharingBenchmarkReport.OperationType.VerifyPooledShares:
                    case SecretSharingBenchmarkReport.OperationType.ShareGeneration:
                    case SecretSharingBenchmarkReport.OperationType.VerifyShares:
                    case SecretSharingBenchmarkReport.OperationType.DecryptShares:
                    case SecretSharingBenchmarkReport.OperationType.ShareGenerationRandomSecret:
                    case SecretSharingBenchmarkReport.OperationType.ShareGenerationSpecialSecret:
                    case SecretSharingBenchmarkReport.OperationType.InitProtocol:
                    default:
                        selector = delegate(SecretSharingBenchmarkReport po) { return po.n; };

                        break;
                }
                var temp = reportGroup.Reports.GroupBy(po => selectorTitle != "k" ? po.k : po.n).Select(grp => new
                {
                    Axis = grp.Key,
                    reports = reportGroup.Reports.Where(rpt => (selectorTitle != "k" ? rpt.k : rpt.n) == grp.Key)
                });
                var sortedTemp = temp.OrderByDescending(po => po.Axis);
                foreach (var groupedAxis in sortedTemp)
                {
                    curves.Add(groupedAxis.reports);
                    titles.Add(string.Format("{0}={1}", selectorTitle != "k" ? "k" : "n", groupedAxis.Axis));
                    if (reportGroup.Operation == SecretSharingBenchmarkReport.OperationType.DecryptShares
                        || reportGroup.Operation == SecretSharingBenchmarkReport.OperationType.VerifyPooledShares
                        || reportGroup.Operation == SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction)
                    {
                        break;
                    }
                }

                perfViewer.ExportPerformanceReportsAsPNG_PDF(curves, titles, "",/*String.Format("{0}-Key length:{1}"reportGroup.Operation.ToString(), reportGroup.KeyLength),*/
                    false, true, selector, selectorTitle, po => (double)po.Average() / (double)TimeSpan.TicksPerMillisecond, "Time (ms)",
                    string.Format("{0}perf_{1}_{2}bits", output,reportGroup.Operation.ToString(), reportGroup.KeyLength));
                var comparereport =filteredcompared.Where(po=>po.KeyLength*2==reportGroup.KeyLength && (po.Operation == reportGroup.Operation || (po.Operation == SecretSharingBenchmarkReport.OperationType.ShareGeneration && reportGroup.Operation == SecretSharingBenchmarkReport.OperationType.ShareGenerationRandomSecret)));
                
                pdfreport.GenBenchmarkDoc(string.Format("{0}{1}bit_n{2}_k{3}_{4}.pdf", output, report.KeySize, report.N, report.K, reportGroup.Operation.ToString()), reportGroup.Reports,comparereport==null||comparereport.Count()==0?null:comparereport.Single().Reports);
            }
         
             
        }

        static void HandleReportsGenericBenaloh(BenalohPersistanceReport report, string output = "")
        {
            //group by key and handle for each key following outputs
            var selectdDivide = report.Reports.Select(po => po.divideItem).GroupBy(grp => grp.KeyLength).Select(grp => new { KeyLength=grp.Key, Operation = OperationType.DivideSecret, reports = grp.Take(grp.Count()) });
            var selectdReconstruct = report.Reports.Select(po => po.reconstructItem).GroupBy(grp => grp.KeyLength).Select(grp => new { KeyLength = grp.Key, Operation = OperationType.ReconstructSecret, reports = grp.Take(grp.Count()) });;

            PDFGenerator pdfreport = new PDFGenerator();


            //selectdDivide = selectdDivide.Concat(selectdReconstruct);
            foreach (var reportGroup in selectdReconstruct.Concat(selectdDivide))
            {
                PerormanceAnalysisForm perfViewer = new PerormanceAnalysisForm();
                var curves = new List<IEnumerable<BenalohLeichterBenchmarkReport>>();
                //curves.Add(reportGroup.Reports);
                var titles = new List<String>() { reportGroup.KeyLength.ToString() };
                Func<BenalohLeichterBenchmarkReport, long> selector=null;
                string selectorTitle = "n";
                switch (reportGroup.Operation)
                {

                    case OperationType.DivideSecret:
                    case OperationType.ReconstructSecret:
                        selectorTitle = "Length of the access structure";
                        selector = delegate(BenalohLeichterBenchmarkReport po) { return po.Access.Where(pr => char.IsDigit(pr)).Count(); };

                        break;
                }

                var mergedAccssesWithSameLength=reportGroup.reports.GroupBy(selector).Select(grp => new BenalohLeichterBenchmarkReport
                 (){
                     KeyLength = grp.First().KeyLength,
                    Access= grp.First().Access,
                    ElapsedTicks = accumulateElapsedTime(grp.Count(),grp.Select(po=>po.ElapsedTicks)),
                }).OrderBy(po=>po.Access.Where(pr => char.IsDigit(pr)).Count());

                curves.Add(mergedAccssesWithSameLength);
                perfViewer.ExportPerformanceReportsAsPNG_PDF(curves, titles, 
                    "",//String.Format("{0}-Key length:{1}", reportGroup.Operation.ToString(), reportGroup.KeyLength),
                    false, true, selector, selectorTitle, po => (double)po.Average() / (double)TimeSpan.TicksPerMillisecond, "time(ms)",
                    string.Format("{0}/perf_{1}_{2}bits",output, reportGroup.Operation.ToString(), reportGroup.KeyLength));
            }
            //pdfreport.GenBenchmarkDoc(string.Format("{0}{1}bit_n{2}_k{3}_recon_chunk{4}bit.pdf", output, report.KeySize, report.N, report.K, chunk * 8), queryreconrep);
            //pdfreport.GenBenchmarkDoc(string.Format("{0}{1}bit_n{2}_k{3}_gen_chunk{4}bit.pdf", output, report.KeySize, report.N, report.K, chunk * 8), querygenrep);
        }

        static long[] accumulateElapsedTime(int count, IEnumerable<long[]> elapseds)
        {
            var re = new List<long>();
            for (int i = 0; i < count; i++)
            {
                re.AddRange(elapseds.ElementAt(i).Skip(2));
            }
            return re.ToArray();

        }
        static void Main(string[] args)
        {
            //BenalohLeichterBenchmark blb = new BenalohLeichterBenchmark();
            //blb.ExploreAllPossibleAccessStructures(new QualifiedSubset("p1^p2^p3").Parties);

            //Console.Read();
            //return;

            if (args.Length == 0) {printHelp(); return ;}
            string op = args[0].Trim().ToLower();
            var filePath ="";

            if (op == "benchmark-shamir")
            {
               
                var minN = Convert.ToInt32(args[1]);
                var maxN = Convert.ToInt32(args[2]);
                var minK = Convert.ToInt32(args[3]);
                var maxK = Convert.ToInt32(args[4]);
                var step = Convert.ToInt32(args[5]);
                var Keys = args[6].Split(',');
                var primePath = args[7];
                var primeXML = new XMLSerializer<PrimePersistanceReport>();
                var loadedPrimes = primeXML.Deserialize(primePath.Replace("output=",""));
                //var primes = 
                
                filePath = BenchmarkToPersistenceStorage(minN,maxN,minK,maxK,step,Keys,SecretSharingBenchmarkReport.SSAlgorithm.Shamir,loadedPrimes.Primes);
                Console.WriteLine(string.Format("Benchmarking is done!\nResults are dumped to {0}", filePath));
            }
            else if (op == "generate-prime")
            {
                var sizes = args[1].Split(',');
                var count = Convert.ToInt32(args[2]);
                var path = args[3].Replace("output=","");
                BenchmarkPrime gprime = new BenchmarkPrime();
                var re = gprime.BenchmarkPrimes(sizes, count);
                var ser = new XMLSerializer<PrimePersistanceReport>();
                PrimePersistanceReport primeXml = new PrimePersistanceReport();
                primeXml.Primes = re;
                ser.Serialize(primeXml, path);
            }
            else if (op == "benchmark-schoen")
            {

                var minN = Convert.ToInt32(args[1]);
                var maxN = Convert.ToInt32(args[2]);
                var minK = Convert.ToInt32(args[3]);
                var maxK = Convert.ToInt32(args[4]);
                var step = Convert.ToInt32(args[5]);
                var Keys = args[6].Split(',');
                var primePath = args[7];
                var primeXML = new XMLSerializer<PrimePersistanceReport>();
                var loadedPrimes = primeXML.Deserialize(primePath);

                filePath = BenchmarkToPersistenceStorage(minN, maxN, minK, maxK, step, Keys, SecretSharingBenchmarkReport.SSAlgorithm.Schoenmakers, loadedPrimes.Primes);
                Console.WriteLine(string.Format("Benchmarking is done!\nResults are dumped to {0}", filePath));
            }
            else if (op == "benchmark-benaloh")
            {
                var access = new QualifiedSubset(args[1]);
                var iterate = Convert.ToInt32(args[2]);
                var Keys = args[3].Split(',');
                var type = args[4];
                List<LoadedPrimeNumber> primes = null;
                if (args.Count() > 5)
                {
                    var primeXML = new XMLSerializer<PrimePersistanceReport>();
                    var loadedPrimes = primeXML.Deserialize(args[5]);
                    primes = loadedPrimes.Primes;
                }
                if (string.IsNullOrEmpty(type) || (type!="scheme" && type !="share")) throw new Exception("scheme or share please define");
                filePath = BenchmarkBenalohToPersistenceStorage(access.Parties, Keys,iterate,type,primes);
                Console.WriteLine(string.Format("Benchmarking is done!\nResults are dumped to {0}", filePath));
            }
            else if (op == "benchmarkprime")
            {
                var minN = Convert.ToInt32(args[1]);
                var maxN = Convert.ToInt32(args[2]);
                var minK = Convert.ToInt32(args[3]);
                var maxK = Convert.ToInt32(args[4]);
                var step = Convert.ToInt32(args[5]);
                var Keys = args[6].Split(',');

                filePath = BenchmarkPrimeToPersistenceStorage(minN, maxN, minK, maxK, step, Keys);
                Console.WriteLine(string.Format("Benchmarking is done!\nResults are dumped to {0}", filePath));
            }
            else if (op == "report-shamir" || op == "report-schoen")
            {
                if (args.Length == 1 || string.IsNullOrEmpty( args[1]) || !File.Exists(args[1])) {
                    printHelp();
                    return ;
                }
                filePath = args[1];
                var outputPath = args[2].Replace("output=","");
                var comparepath = "";
               
                 var ser = new XMLSerializer<PersistanceReport>();
                 PersistanceReport compareReport = null;  
                if (args.Length > 2)
                 {
                     comparepath = args[3];
                    compareReport = ser.Deserialize(comparepath);
                 }
               
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
                    grep.N = item.Max(po => po.n);// report.N;
                    grep.K = item.Max(po => po.k);// report.K;
                    grep.Step = report.Step;
                    grep.Iterate = report.Iterate;

                    if (op == "report-shamir")
                    {
                        HandleReports(grep, outputPath);
                    }
                    else
                    {
                        HandleReportsGeneric(grep, outputPath, compareReport);
                        
                        //Generate aggeragted decrypt + verify poold + reconstruct
                        if (item.Key / 2 >= 128)
                        {
                            var compreport = compareReport.Reports.Where(po => po.keyLength == item.Key / 2 && po.Operation == SecretSharingBenchmarkReport.OperationType.SecretReconstruction);
                            var printImroves = true;
                            var pdfreport = new PDFGenerator();
                            pdfreport.GenAggreagativeBenchmarkDoc("reconstruct-aggregative-" + item.Key + "bit.pdf", item.Key / 2, printImroves, compreport == null || compreport.Count() == 0 ? null : compreport);
                        }
                    }
                }
                Console.WriteLine("Report files are generated in current path... Old files has been replaced.");
            }
            else if (op == "report-benaloh")
            {
                if (args.Length == 1 || string.IsNullOrEmpty(args[1]) || !File.Exists(args[1]))
                {
                    printHelp();
                    return;
                }
                filePath = args[1];
                var outputPath = args[2].Replace("output=", "");
                var type = args[3];
                if (string.IsNullOrEmpty(type) || (type != "scheme" && type != "share")) throw new Exception("scheme or share please define");
               
                var ser = new XMLSerializer<BenalohPersistanceReport>();
                //var filePath = "SerializedReports-n100-k60-iterate100.xml";
                var report = ser.Deserialize(filePath);
                if (type == "share")
                {
                    HandleBenalohReport(report, outputPath, type);
                }
                else
                {
                    HandleReportsGenericBenaloh(report, outputPath);
                }
                Console.WriteLine("Report files are generated in current path... Old files has been replaced.");
            }
            else if (op == "merge")
            {
                var ser = new XMLSerializer<PersistanceReport>();
                PersistanceReport mergedReports = new PersistanceReport();
                mergedReports.Reports = new List<SecretSharingBenchmarkReport>();
                var outputPath ="";
                foreach (var file in args)
                {
                    if (file == "merge") continue;
                    if (file.Contains("output="))
                    {
                        outputPath = file.Replace("output=", "");
                        continue;
                    }
                    var report = ser.Deserialize(file);
                    mergedReports.Reports.AddRange(report.Reports.ToList());
                    mergedReports.Step = report.Step;
                    mergedReports.Iterate = report.Iterate;
                }
                if(!string.IsNullOrEmpty(outputPath)){
                    ser.Serialize(mergedReports, outputPath);
                    Console.WriteLine("mergerd file suceffully geenrated {0}", outputPath);
                }
            }
            Console.ReadLine();
        }

        private static string BenchmarkBenalohToPersistenceStorage(List<Trustee> participants, string[] Keys, int iterate,string type,List<LoadedPrimeNumber> loadedPrimes)
        {
            var benalohBenchmark = new BenalohLeichterBenchmark();
            var results = new List<BenalohLeichterBenchmarkReportSet>();
            var allAccesses = BenalohLeichterBenchmark.ExploreAllPossibleAccessStructures(participants);
            foreach (var key in Keys)
            {
                results.AddRange( benalohBenchmark.BenchmarkAllAccesses(participants, key, iterate, allAccesses,type,loadedPrimes));
            }
            BenalohPersistanceReport bpr = new BenalohPersistanceReport();
            bpr.Reports = results;
            var serializer = new XMLSerializer<BenalohPersistanceReport>();
            string path = String.Format("Benaloh-SerializedReport-{0}.xml", DateTime.Now.Ticks);
            serializer.Serialize(bpr, path);
            return path;
        }

        private static string BenchmarkToPersistenceStorage(int MinN, int MaxN, int MinK, int MaxK, int step, string[] Keys
            ,SecretSharing.Benchmark.SecretSharingBenchmarkReport.SSAlgorithm alg, List<LoadedPrimeNumber> loadedPrimes )
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



            //int MaxN = 100;
            //int MaxK = 60;
            //int step = 5;
            int iterate=0;
            
            //define the key size to experiment here
            //String key = benchmark.key256bit;
            var reports = new List<SecretSharingBenchmarkReport>();
            if (alg == SecretSharingBenchmarkReport.SSAlgorithm.Shamir)
            {
                iterate = 100;
                ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
                 reports = benchmark.BenchmarkAllKeysWithChunkSize(Chunks, MinN, MaxN, MinK, MaxK, Keys, step, SecretSharingBenchmarkReport.OperationType.ShareGeneration
                     | SecretSharingBenchmarkReport.OperationType.SecretReconstruction
                   , iterate,loadedPrimes);
            }
            else if (alg == SecretSharingBenchmarkReport.SSAlgorithm.Schoenmakers)
            {
                SchoenmakersBenchmark benchmark = new SchoenmakersBenchmark();
                iterate = 10;
                reports = benchmark.BenchmarkAllKeys( MinN, MaxN, MinK, MaxK, Keys, step, SecretSharingBenchmarkReport.OperationType.InitProtocol| 
                    SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction|SecretSharingBenchmarkReport.OperationType.ShareGenerationRandomSecret
                    |SecretSharingBenchmarkReport.OperationType.ShareGenerationSpecialSecret|SecretSharingBenchmarkReport.OperationType.SpecialSecretReconstruction
                    |SecretSharingBenchmarkReport.OperationType.VerifyPooledShares|SecretSharingBenchmarkReport.OperationType.VerifyShares
                    |SecretSharingBenchmarkReport.OperationType.DecryptShares
                   , iterate,loadedPrimes);
            }
            var serializer = new XMLSerializer<PersistanceReport>();
            PersistanceReport report = new PersistanceReport();
            report.Reports = reports.ToList();
            report.N = MaxN;
            report.K = MaxK;
            report.Iterate = iterate;
            report.Step = step;
            //report.KeySize = key.Length * 8;

            string filePath = string.Format("SerializedReports-n{0}-k{1}-iterate{2}-alg{3}-{4}.xml"
                , /*key.Length * 8*/ MaxN, MaxK, iterate,alg.ToString(),DateTime.Now.Ticks.ToString());
            serializer.Serialize(report, filePath);
            return filePath;
        }
        private static string BenchmarkPrimeToPersistenceStorage(int MinN, int MaxN, int MinK, int MaxK, int step, string[] Keys)
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



            //int MaxN = 100;
            //int MaxK = 60;
            //int step = 5;
            int iterate = 100;
            ShamirAntixBenchmark benchmark = new ShamirAntixBenchmark();
            //define the key size to experiment here
            //String key = benchmark.key256bit;

            var reports = benchmark.BenchmarkPrimeWithChunkSize(Chunks, MinN, MaxN, MinK, MaxK, Keys, step, SecretSharingBenchmarkReport.OperationType.ShareGeneration
                 | SecretSharingBenchmarkReport.OperationType.SecretReconstruction
               , iterate);
            var serializer = new XMLSerializer<PersistanceReport>();
            PersistanceReport report = new PersistanceReport();
            report.Reports = reports.ToList();
            report.N = MaxN;
            report.K = MaxK;
            report.Iterate = iterate;
            report.Step = step;
            //report.KeySize = key.Length * 8;

            string filePath = string.Format("SerializedReports-prime-n{0}-k{1}-iterate{2}.xml"
                , /*key.Length * 8*/ MaxN, MaxK, iterate);
            serializer.Serialize(report, filePath);
            return filePath;
        }

        static void printHelp()
        {
            Console.WriteLine(@"
            Instruction to use the benchmarker:
            benchmarkprime minN maxN minK maxK step Keys
            benchmark-shamir minN maxN minK maxK step Keys
            benchmark-schoen minN maxN minK maxK step Keys
            report [filepath] output=[output directory]: to generate pdf & report files
            merge [file1] [file2] [fileN] output=[filePath]
            Press any keys to exit..");
            Console.ReadLine();
        }
    }
}
