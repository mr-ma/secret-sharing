using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pUnit;
using SecretSharing.Test;


namespace SecretSharing.Benchmark
{
    [ProfileClass]
    public class ShamirBenchmark
    {

        String key64bit = "12345678";
        String key128bit = "1234567812345678";
        String key256bit = "12345678123456781234567812345678";
        String key512bit = "1234567812345678123456781234567812345678123456781234567812345678";

        [ProfileMethod(100)]
        public void GenerateShare64bitKey()
        {
            int n = 10;
            int k = 5;
            var score = new SecretSharingCoreTests();
            score.TestDivideSecret(n, k, key64bit);
        }
         [ProfileMethod(100)]
        public void GenerateShare128bitKey()
        {
            int n = 10;
            int k = 5;
            var score = new SecretSharingCoreTests();
            score.TestDivideSecret(n, k, key128bit);
        }
         [ProfileMethod(100)]
         public void GenerateShare256bitKey()
         {
             int n = 10;
             int k = 5;
             var score = new SecretSharingCoreTests();
             score.TestDivideSecret(n, k, key256bit);
         }
         [ProfileMethod(100)]
         public void GenerateShare512bitKey()
         {
             int n = 10;
             int k = 5;
             var score = new SecretSharingCoreTests();
             score.TestDivideSecret(n, k, key512bit);
         }
    }
}
