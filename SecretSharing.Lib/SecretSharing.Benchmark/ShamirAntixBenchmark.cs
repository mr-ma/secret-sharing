using SecretSharing.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antix.Testing;
using System.Threading;
namespace SecretSharing.Benchmark
{
    public class SecretSharingBenchmarkReport
    {
        public int n;
        public int k;
        public TimeSpan avg;
        public int keyLength;
    }
    public class ShamirAntixBenchmark
    {
        string[] keys = new string[]{"12345678"
            ,"1234567812345678"
            ,"12345678123456781234567812345678"
           // ,"1234567812345678123456781234567812345678123456781234567812345678"
        };
        String key64bit = "12345678";
        String key128bit = "1234567812345678";
        String key256bit = "12345678123456781234567812345678";
        String key512bit = "1234567812345678123456781234567812345678123456781234567812345678";

        
        public IEnumerable< String> BenchmarkMe()
        {
            var score = new SecretSharingCoreTests();
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();

            for (int n = 10; n <= 50; n += 5)
            {
                //k can not be bigger than n
                for (int k = 1; k <= n; k++)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        var re = Antix.Testing.Benchmark.Run(() => score.TestDivideSecret(n, k * 5, keys[i]), 10);
                        results.Add(new SecretSharingBenchmarkReport() { n = n, k = k * 5, avg = re.Average, keyLength = keys[i].Length * 8 });
                    }
                }
            }
            var orderedResults = results.OrderBy(po => po.keyLength).ThenBy(po => po.n).ThenBy(po => po.k); 


            return orderedResults.Select(po => String.Format("keylength:{0}  n:{1} k:{2}  avg:{3}", po.keyLength, po.n, po.k, po.avg));
        }
    }
}
