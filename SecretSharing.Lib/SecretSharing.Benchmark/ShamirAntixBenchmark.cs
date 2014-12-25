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
            ShareGeneration =1,
            SecretReconstruction =2,
        }
        public byte chunkSize;
        public int n;
        public int k;
        public TimeSpan avg;
        public int keyLength;
        public OperationType Operation;

        public override string ToString()
        {
            return String.Format("keylength:{0} chunkSize:{1}  n:{2} k:{3}  avg:{4} Operation: {5}", keyLength,chunkSize, n, k, avg, Operation.ToString());
        }
    }
    public class ShamirAntixBenchmark
    {
        string[] keys = new string[]{"12345678"
            ,"1234567812345678"
            ,"12345678123456781234567812345678"
            ,"1234567812345678123456781234567812345678123456781234567812345678"
        };
        byte[] Chunks = new Byte[]{
            1,2,4,8,16,32
        };
        public String key64bit = "12345678";
        public String key128bit = "1234567812345678";
        public string key256bit = "12345678123456781234567812345678";
        public String key512bit = "1234567812345678123456781234567812345678123456781234567812345678";

        
       

        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkMeWithChunkSize(
            byte chunkSize, int MaxN, int MaxK, int step,SecretSharingBenchmarkReport.OperationType operation,string key = "12345678",int iterate=1)
        {
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            for (int n = 5; n <= MaxN; n += step)
            {
                //k can not be bigger than n
                for (int k = 1; k <= n && k<=MaxK;)
                {

                    for (int iteratechunk = 0; iteratechunk < Chunks.Length; iteratechunk++)
                    {
                       // for (int i = 0; i < keys.Length; i++)
                        {
                            if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.ShareGeneration))
                            {
                                var redivide = Antix.Testing.Benchmark.Run(() =>
                                   DivideSecretWithChunkSizeWrapper(n, k, Chunks[iteratechunk], key/*keys[i]*/)
                                   , iterate);
                               
                                results.Add(new SecretSharingBenchmarkReport() { n = n, chunkSize = Chunks[iteratechunk], k = k, avg = redivide.Average, keyLength = key/*s[i]*/.Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.ShareGeneration });
                            }
                            if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.SecretReconstruction))
                            {
                                var shares = DivideSecretWithChunkSizeWrapper(n, k, Chunks[iteratechunk], key /*keys[i]*/);

                                var reconstruct = Antix.Testing.Benchmark.Run(() =>
                              ReconstructSecretWithChunkSizeWrapper(shares,k, Chunks[iteratechunk])
                              , iterate);
                                results.Add(new SecretSharingBenchmarkReport() { n = n, chunkSize = Chunks[iteratechunk], k = k, avg = reconstruct.Average, keyLength = key/*s[i]*/.Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.SecretReconstruction });
                            }
                            Console.WriteLine("Iteration info: n:{0} k:{1} keySize:{2} chunkSize(bits):{3}", n,k, key/*s[i]*/.Length * 8, Chunks[iteratechunk]*8);
                        }
                    }
                    if (k == 1) k = step;
                    else k += step;
                }
            }
            var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k).ThenBy(po=>po.chunkSize);


            return orderedResults; //orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }


        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkMeWithChunkFloatSize(
          int MaxN, int MaxK, int step, SecretSharingBenchmarkReport.OperationType operation, string key = "12345678", int iterate = 1)
        {
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            for (int n = 5; n <= MaxN; n += step)
            {
                //k can not be bigger than n
                for (int k = 1; k <= n && k <= MaxK; )
                {

                   // for (int iteratechunk = 0; iteratechunk < Chunks.Length; iteratechunk++)
                    {
                        // for (int i = 0; i < keys.Length; i++)
                        {
                            if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.ShareGeneration))
                            {
                                var redivide = Antix.Testing.Benchmark.Run(() =>
                                   DivideSecretWithChunkSizeWrapper(n, k, (byte)key.Length, key/*keys[i]*/)
                                   , iterate);

                                results.Add(new SecretSharingBenchmarkReport() { n = n, chunkSize = (byte)key.Length, k = k, avg = redivide.Average, keyLength = key/*s[i]*/.Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.ShareGeneration });
                            }
                            if (operation.HasFlag(SecretSharingBenchmarkReport.OperationType.SecretReconstruction))
                            {
                                var shares = DivideSecretWithChunkSizeWrapper(n, k, (byte)key.Length, key /*keys[i]*/);

                                var reconstruct = Antix.Testing.Benchmark.Run(() =>
                              ReconstructSecretWithChunkSizeWrapper(shares,k, (byte)key.Length)
                              , iterate);
                                results.Add(new SecretSharingBenchmarkReport() { n = n, chunkSize = (byte)key.Length, k = k, avg = reconstruct.Average, keyLength = key/*s[i]*/.Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.SecretReconstruction });
                            }
                            Console.WriteLine("Iteration info: n:{0} k:{1} keySize:{2} chunkSize(bits):{3}", n, k, key/*s[i]*/.Length * 8, (byte)key.Length * 8);

                            GC.Collect();
                        }
                    }
                    if (k == 1) k = step;
                    else k += step;
                }
            }
            var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k).ThenBy(po => po.chunkSize);


            return orderedResults; //orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }


        public void BenchmarkMeWithChunkSize(
           byte chunkSize, int MaxN, int MaxK, int step, string PathToReportFile,string key, int iterate = 1)
        {
            File.Delete(PathToReportFile);

            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            for (int n = 10; n <= MaxN; n += step)
            {
                //k can not be bigger than n
                for (int k = 1; k <= n && k < MaxK; )
                {

                    for (int iteratechunk = 0; iteratechunk < Chunks.Length; iteratechunk++)
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            var redivide = Antix.Testing.Benchmark.Run(() =>
                               DivideSecretWithChunkSizeWrapper(n, k, Chunks[iteratechunk], keys[i])
                               , iterate);
                            var shares = DivideSecretWithChunkSizeWrapper(n, k, Chunks[iteratechunk], keys[i]);
                            var reconstruct = Antix.Testing.Benchmark.Run(() =>
                          ReconstructSecretWithChunkSizeWrapper(shares,k, Chunks[iteratechunk])
                          , iterate);
                           // results.Add(
                            var gen = new SecretSharingBenchmarkReport() { n = n, chunkSize = Chunks[iteratechunk], k = k, avg = redivide.Average, keyLength = keys[i].Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.ShareGeneration };
                            //results.Add(
                            var con = new SecretSharingBenchmarkReport() { n = n, chunkSize = Chunks[iteratechunk], k = k, avg = reconstruct.Average, keyLength = keys[i].Length * 8, Operation = SecretSharingBenchmarkReport.OperationType.SecretReconstruction };
                            Console.WriteLine("Iteration info: n:{0} k:{1} keySize:{2} chunkSize(bits):{3}", n, k, keys[i].Length * 8, Chunks[iteratechunk] * 8);
                            File.AppendAllText(PathToReportFile, gen.ToString()+Environment.NewLine);
                            File.AppendAllText(PathToReportFile, con.ToString() + Environment.NewLine);
                        }
                    }
                    if (k == 1) k = step;
                    else k += step;
                }
            }
           // var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k).ThenBy(po => po.chunkSize);


           // return orderedResults; //orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }


        public void BenchmarkMeWithChunkSizeFixedParameters()
        {
            var n = 10;
            var k = 3;
            var iterate = 5;

            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            for (int i = 0; i < 1000000; i++)
            {
                var re = Antix.Testing.Benchmark.Run(() => DivideSecretWithChunkSizeWrapper(n, k * 5, 8, keys[0]), iterate);
                GC.Collect();
            }
                // results.Add(new SecretSharingBenchmarkReport() { n = n, k = k * 5, avg = re.Average, keyLength = keys[0].Length * 8 });
           // var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k);


            // orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }



        public List<IShareCollection> DivideSecretWithChunkSizeWrapper(int n, int k, byte ChunkSize, String Secret)
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            var byteSecret = Encoding.UTF8.GetBytes(Secret.ToCharArray());
            return shamir.DivideSecret(k, n, byteSecret, ChunkSize);
        }

        public void ReconstructSecretWithChunkSizeWrapper(List<IShareCollection> shares,int k, byte chunkSize)
        {
            SecretSharingCore.Algorithms.Shamir shamir = new SecretSharingCore.Algorithms.Shamir();
            //assign
            var reconSecret = Encoding.UTF8.GetString(shamir.ReconstructSecret(shares.GetRange(0,k), chunkSize));

        }
    }
}
