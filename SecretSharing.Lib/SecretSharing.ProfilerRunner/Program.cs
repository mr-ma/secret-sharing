using pUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.ProfilerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProfileRunner runner = new ProfileRunner();
            //runner.Run();

            Benchmark.ShamirAntixBenchmark benchmark = new Benchmark.ShamirAntixBenchmark();
            //var re =  benchmark.BenchmarkMeWithChunkSize();
            //foreach (var item in re)
            //{
            //    Console.WriteLine(item);
            //}
            benchmark.BenchmarkMeWithChunkSizeFixedParameters();
            Console.ReadLine();
            //System.IO.File.WriteAllLines("benchmarkresultsNew.txt", re);
        }
    }
}
