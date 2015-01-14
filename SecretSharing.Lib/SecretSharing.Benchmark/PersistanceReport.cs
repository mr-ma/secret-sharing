using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Benchmark
{
    public class PersistanceReport
    {
        public List<SecretSharingBenchmarkReport> Reports { get; set; }
        public int N { get; set; }
        public int K { get; set; }

        public int KeySize { get; set; }
        public int Iterate { get; set; }


        public int Step { get; set; }
    }
}
