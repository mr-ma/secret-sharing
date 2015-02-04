using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antix.Testing;
using System.Threading;
using System.IO;
using SecretSharingCore.Common;
namespace SecretSharing.Benchmark
{
    public class SecretSharingBenchmarkReport
    {
        [Flags]
        public enum OperationType
        {
            ShareGeneration = 1,
            SecretReconstruction = 2,
        }
        public int chunkSize;
        public int n;
        public int k;
        public double TotalElapsedMilliseconds;
        public int keyLength;
        public OperationType Operation;
        public double primeGenerationTime;
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
    public class ShamirAntixBenchmark
    {
        string[] keys = new string[]{
            /*64bits*/"12345678"
            /*128bits*/,"1234567812345678"
            /*256bits*/,"12345678123456781234567812345678"
            /*512bits*/,"1234567812345678123456781234567812345678123456781234567812345678"
            //,"12345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678"
        //,"1234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678"
       
        };

        public String key64bit = "12345678";
        public String key128bit = "1234567812345678";
        public string key256bit = "12345678123456781234567812345678";
        public String key512bit = "1234567812345678123456781234567812345678123456781234567812345678";
        public String key1024bit = "12345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678";


        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkAllKeysWithChunkSize(int[] chunkSize, 
            int MaxN, int MaxK, int step, 
            SecretSharingBenchmarkReport.OperationType operation,
             int iterate = 1)
        {
            var reports = new List<SecretSharingBenchmarkReport>();
            for (int i = 0; i < keys.Length; i++)
            {
                reports.AddRange(BenchmarkKeyWithChunkSize(chunkSize,MaxN,MaxK,step,operation,keys[i],iterate));
            }
            return reports;
        }
        object lockMe = new object();
        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkKeyWithChunkSize(
            int[] chunkSize, int MaxN, int MaxK, int step,
            SecretSharingBenchmarkReport.OperationType operation, string key = "12345678", int iterate = 1)
        {
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            //Parallel.For(5, MaxN, n =>
            //{
                for (int n = 5; n <= MaxN; n += step)
                {
                //k can not be bigger than n

                //Parallel.For(1, MaxK, k =>
                //{
                    for (int k = 1; k <= n && k <= MaxK; )
                    {

                        for (int iteratechunk = 0; iteratechunk < chunkSize.Length; iteratechunk++)
                        {
                            ///skip if the chunk is bigger than the secret
                            if (chunkSize[iteratechunk] * 8 > key.Length * 8) break;

                            List<IShareCollection> shares = null;
                            //if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.ShareGeneration))
                            //{
                            double primeTime = 0;

                            var redivide = Antix.Testing.Benchmark.Run(() =>
                               shares = DivideSecretWithChunkSizeWrapper(n, k, chunkSize[iteratechunk], key, ref primeTime/*keys[i]*/)
                               , iterate);
                            //In the function
                            lock (lockMe)
                            {
                                results.Add(new SecretSharingBenchmarkReport()
                                {
                                    n = n,
                                    chunkSize = chunkSize[iteratechunk],
                                    k = k,
                                    TotalElapsedMilliseconds = redivide.Average.TotalMilliseconds,
                                    keyLength = key/*s[i]*/.Length * 8,
                                    Operation = SecretSharingBenchmarkReport.OperationType.ShareGeneration,
                                    primeGenerationTime = primeTime / iterate
                                });
                            }
                            //}
                            if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.SecretReconstruction))
                            {
                                var reconstruct = Antix.Testing.Benchmark.Run(() =>
                              ReconstructSecretWithChunkSizeWrapper(shares, k, chunkSize[iteratechunk])
                              , iterate);
                                lock (lockMe)
                                {
                                    results.Add(new SecretSharingBenchmarkReport()
                                    {
                                        n = n,
                                        chunkSize = chunkSize[iteratechunk],
                                        k = k,
                                        TotalElapsedMilliseconds = reconstruct.Average.TotalMilliseconds,
                                        keyLength = key/*s[i]*/.Length * 8,
                                        Operation = SecretSharingBenchmarkReport.OperationType.SecretReconstruction
                                    });
                                }
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


            return orderedResults; //orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }


  
        public List<IShareCollection> DivideSecretWithChunkSizeWrapper(int n, int k, int ChunkSize, String Secret, ref double PrimeGenerationTime)
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var byteSecret = Encoding.UTF8.GetBytes(Secret.ToCharArray());
            double? primeElapsed = 0;
            var a = shamir.DivideSecret(k, n, byteSecret, ChunkSize,ref primeElapsed);
            PrimeGenerationTime += shamir.GetPrimeGenerationTime();
            return a;
        }

        public void ReconstructSecretWithChunkSizeWrapper(List<IShareCollection> shares, int k, int chunkSize)
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            //assign
            var reconSecret = Encoding.UTF8.GetString(shamir.ReconstructSecret(shares.GetRange(0, k), chunkSize));

        }
    }
}
