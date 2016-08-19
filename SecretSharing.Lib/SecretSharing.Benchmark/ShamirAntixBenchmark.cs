using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecretSharingCore.Common;
using System.Diagnostics;
using SecretSharingCore;
namespace SecretSharing.Benchmark
{
    [Serializable]
    public class SecretSharingBenchmarkReport
    {
        public enum OperationType
        {
            Example = 0,
            ShareGeneration = 1,
            SecretReconstruction = 2,
            PrimeGneration = 3,
            VerifyShares = 4,
           // VerifyDecryption=5,
            DecryptShares = 6,
            VerifyPooledShares = 7,
            ShareGenerationRandomSecret = 8,
            ShareGenerationSpecialSecret = 9,
            RandomSecretReconstruction = 10,
            SpecialSecretReconstruction = 11,
            InitProtocol = 12,
        }
        public enum SSAlgorithm
        {
            Shamir,
            Schoenmakers,
            BenalohLeichter,
        }
        public int chunkSize;
        public int n;
        public int k;
        public double TotalElapsedMilliseconds;
        public int keyLength;
        public OperationType Operation;
        public long[] ElapsedTicks { get; set; }
#if calcPrimeTime
        public double primeGenerationTime;
#endif
        //public long avgTotalMiliseconds
        //{
        //    get
        //    {
        //        return Convert.ToInt64(this.TotalElapsedMilliseconds);
        //    }
        //}
        public override string ToString()
        {
            return String.Format("keylength:{0} chunkSize:{1}  n:{2} k:{3}  avg:{4} Operation: {5}", keyLength, chunkSize, n, k, TotalElapsedMilliseconds, Operation.ToString());
        }
    }
    public class ShamirAntixBenchmark //: SecretSharing.Benchmark.IBenchmark
    {
        string[] keys = new string[]{
            /*64bits*/"12345678"
            /*128bits*/,"1234567812345678"
            /*256bits*/,"12345678123456781234567812345678",
            /*512bits*/"1234567812345678123456781234567812345678123456781234567812345678"
            //,"12345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678"
        //,"1234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678"
       
        };

        public String key64bit = "12345678";
        public String key128bit = "1234567812345678";
        public string key256bit = "12345678123456781234567812345678";
        public String key512bit = "1234567812345678123456781234567812345678123456781234567812345678";
        public String key1024bit = "12345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678";

        public List<IShareCollection> DivideSecretWithChunkSizeWrapper(int n, int k, int ChunkSize, String Secret,ref List<long> ElapsedTicks
#if calcPrimeTime
            , ref double PrimeGenerationTime,ref double allTime
#endif
            )
        {
            if (n == 0) n = 1;
            if (k == 0) k = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var byteSecret = Encoding.UTF8.GetBytes(Secret.ToCharArray());
#if calcPrimeTime 
            double primeElapsed = 0;
            var a = shamir.DivideSecret(k, n, byteSecret, ChunkSize,ref primeElapsed);
            PrimeGenerationTime +=Math.Pow( primeElapsed,2.0); 
            allTime +=Math.Pow( (double)sw.ElapsedTicks,2.0); 
#else
            var a = shamir.DivideSecret(k, n, byteSecret,ChunkSize);
#endif
            sw.Stop();
            ElapsedTicks.Add(sw.ElapsedTicks);
            return a;
        }

        public void GeneratePrimeWithChunkSizeWrapper(int ChunkSize,ref List<Int64> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SecretSharingCore.PrimeGenerator pg = new SecretSharingCore.PrimeGenerator();
            pg.GenerateRandomPrime(ChunkSize);
            sw.Stop();
            elapsedTicks.Add(sw.ElapsedTicks);
        }

        public void ReconstructSecretWithChunkSizeWrapper(List<IShareCollection> shares, int k, int chunkSize, ref List<long> elapsedTicks,string secret)
        {
            if (k == 0) k = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            //assign
            var reconSecret = Encoding.UTF8.GetString(shamir.ReconstructSecret(shares.GetRange(0, k), chunkSize));
            sw.Stop();
            if (reconSecret != secret) throw new Exception("Not able to recon secret!");
            elapsedTicks.Add(sw.ElapsedTicks);
        }

        public List<SecretSharingBenchmarkReport> BenchmarkAllKeysWithChunkSize(int[] chunkSize, int MinN, int MaxN,
            int MinK, int MaxK, string[] Keys,
            int step, SecretSharingBenchmarkReport.OperationType operation, int iterate,List<LoadedPrimeNumber> loadedPrimes)
        {
            var reports = new List<SecretSharingBenchmarkReport>();
            for (int i = 0; i < Keys.Length; i++)
            {
                reports.AddRange(BenchmarkKeyWithChunkSize(chunkSize, MinN, MaxN, MinK, MaxK, step, operation, Keys[i], iterate,loadedPrimes));
                //reports.AddRange(BenchmarkPrimeWithChunkSize(chunkSize, MinN, MaxN, MinK, MaxK, step, Keys[i], iterate));
            }
            return reports;
        }
        public List<SecretSharingBenchmarkReport> BenchmarkPrimeWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK, int MaxK, string[] Keys, int step, SecretSharingBenchmarkReport.OperationType operation, int iterate)
        {
            var reports = new List<SecretSharingBenchmarkReport>();
            for (int i = 0; i < Keys.Length; i++)
            {
                reports.AddRange(BenchmarkPrimeWithChunkSize(chunkSize, MinN, MaxN, MinK, MaxK, step, Keys[i], iterate));
            }
            return reports;
        }
        private IEnumerable<SecretSharingBenchmarkReport> BenchmarkKeyWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK,
            int MaxK, int step, SecretSharingBenchmarkReport.OperationType operation, string key, int iterate, List<LoadedPrimeNumber> LoadedPrimes)
        {
            var filteredPrimes = LoadedPrimes.Where(po => po.PrimeSize == key.Length).ToList();
            PrimeGenerator.SetLoadedPrimes(filteredPrimes);
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            //Parallel.For(5, MaxN, n =>
            //{
            for (int n = MinN; n <= MaxN; n += step)
            {
                //k can not be bigger than n

                //Parallel.For(1, MaxK, k =>
                //{
                for (int k = MinK; k <= n && k <= MaxK; )
                {

                    for (int iteratechunk = 0; iteratechunk < chunkSize.Length; iteratechunk++)
                    {
                        ///skip if the chunk is bigger than the secret
                        if (chunkSize[iteratechunk] * 8 > key.Length * 8) break;

                        List<IShareCollection> shares = null;
                        //if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.ShareGeneration))
                        //{
                        double primeTime = 0;
                        double allTime = 0;
                        var ElapsedTicks = new List<long>();

                        filteredPrimes = LoadedPrimes.Where(po => po.PrimeSize == chunkSize[iteratechunk]).ToList();
                        PrimeGenerator.SetLoadedPrimes(filteredPrimes);

                        var redivide = Antix.Testing.Benchmark.Run(() =>
                           shares = DivideSecretWithChunkSizeWrapper(n, k, chunkSize[iteratechunk], key
#if calcPrimeTime
                           , ref primeTime, ref allTime
#endif
                           ,ref ElapsedTicks), iterate);
                        //In the function
                        //var primegenerationTime = primeTime / iterate;
                        //if (allTime < primeTime)
                        //{
                        //    throw new Exception("AVG Prime generation time is bigger than all elapsed time AVG");
                        //}
                        results.Add(new SecretSharingBenchmarkReport()
                        {
                            n = n,
                            chunkSize = chunkSize[iteratechunk],
                            k = k,
                            TotalElapsedMilliseconds =
#if calcPrimeTime
                            Math.Sqrt( (1.0 /(double)iterate)* allTime )/ (double)TimeSpan.TicksPerMillisecond
#else
 redivide.Average.TotalMilliseconds,
#endif
ElapsedTicks = ElapsedTicks.ToArray(),
                            keyLength = key/*s[i]*/.Length * 8,
                            Operation = SecretSharingBenchmarkReport.OperationType.ShareGeneration,
#if calcPrimeTime
                            primeGenerationTime = Math.Sqrt((1.0 / (double)iterate) * primeTime) / (double)TimeSpan.TicksPerMillisecond
#endif
                        });
                        if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.SecretReconstruction))
                        {
                            List<long> elapsedRecons = new List<long>();
                            var reconstruct = Antix.Testing.Benchmark.Run(() =>
                          ReconstructSecretWithChunkSizeWrapper(shares, k, chunkSize[iteratechunk],ref elapsedRecons,key)
                          , iterate);
                            results.Add(new SecretSharingBenchmarkReport()
                            {
                                n = n,
                                chunkSize = chunkSize[iteratechunk],
                                k = k,
                                TotalElapsedMilliseconds = reconstruct.Average.TotalMilliseconds,
                                keyLength = key/*s[i]*/.Length * 8,
                                ElapsedTicks = elapsedRecons.ToArray(),
                                Operation = SecretSharingBenchmarkReport.OperationType.SecretReconstruction
                            });
                        }
                        Console.WriteLine("Iteration info: n:{0} k:{1} keySize:{2} chunkSize(bits):{3}", n, k, key/*s[i]*/.Length * 8, chunkSize[iteratechunk] * 8);
                    }
                    if (k == 1) k = step;
                    else k += step;
                }
                //}
                //});
            }//);
            var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k).ThenBy(po => po.chunkSize);


            return orderedResults;
        }
        
        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkPrimeWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK, int MaxK, int step,string key, int iterate)
        {
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            //Parallel.For(5, MaxN, n =>
            //{
            for (int n = MinN; n <= MaxN; n += step)
            {
                //k can not be bigger than n

                //Parallel.For(1, MaxK, k =>
                //{
                for (int k = MinK; k <= n && k <= MaxK; )
                {

                    for (int iteratechunk = 0; iteratechunk < chunkSize.Length; iteratechunk++)
                    {
                        ///skip if the chunk is bigger than the secret
                        if (chunkSize[iteratechunk] * 8 > key.Length * 8) break;
                        List<Int64> PrimeElapseResults = new List<long>();
                        
                        var redivide = Antix.Testing.Benchmark.Run(() =>
                           GeneratePrimeWithChunkSizeWrapper(chunkSize[iteratechunk], ref PrimeElapseResults), iterate);

                        var skipedAvg = PrimeElapseResults.Skip(10).Aggregate((current, next) => current + next);
                        var EAvg = skipedAvg / (iterate - 10);
                        var sumSqrAvg = PrimeElapseResults.Select(po => Math.Pow((double)po, 2.0)).Aggregate((current, next) => current + next);
                        var rootMeanAvg = Math.Sqrt((1.0 / ((double)iterate)) * sumSqrAvg) / TimeSpan.TicksPerMillisecond;
                        var standardDivSum = PrimeElapseResults.Select(x => Math.Pow(x - redivide.Average.Ticks, 2)).Aggregate((current, next) => current + next);
                        var standardD = Math.Sqrt((1.0 / ((double)iterate)) * standardDivSum) / TimeSpan.TicksPerMillisecond;
                        results.Add(new SecretSharingBenchmarkReport()
                        {
                            n = n,
                            chunkSize = chunkSize[iteratechunk],
                            k = k,
                            TotalElapsedMilliseconds = EAvg/*redivide.Average.TotalMilliseconds*/,
                            keyLength = key/*s[i]*/.Length * 8,
                            Operation = SecretSharingBenchmarkReport.OperationType.PrimeGneration,
                            ElapsedTicks = PrimeElapseResults.ToArray()
                        });
                     
                        Console.WriteLine("Iteration info: n:{0} k:{1} keySize:{2} chunkSize(bits):{3}", n, k, key/*s[i]*/.Length * 8, chunkSize[iteratechunk] * 8);
                    }
                    if (k == 1) k = step;
                    else k += step;
                }
                //}
                //});
            }//);
            var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k).ThenBy(po => po.chunkSize);


            return orderedResults;
        }
   
    
    }
}
