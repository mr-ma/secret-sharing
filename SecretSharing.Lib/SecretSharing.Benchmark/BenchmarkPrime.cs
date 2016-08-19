using SecretSharingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.Benchmark
{
    public class BenchmarkPrime
    {
        public List<LoadedPrimeNumber> BenchmarkPrimes(string[] sizes, int count)
        {
            List<LoadedPrimeNumber> lpn = new List<LoadedPrimeNumber>();
            foreach (var size in sizes)
            {
                lpn.AddRange(GeneratePrimeNumbers(count,Convert.ToInt32( size)));
            }
            return lpn;
        }
        public List<LoadedPrimeNumber> GeneratePrimeNumbers(int count, int size)
        {
            List<LoadedPrimeNumber> primes = new List<LoadedPrimeNumber>();
            PrimeGenerator pg = new PrimeGenerator();
            for (int i = 0; i < count; i++)
            {
                var prime = pg.GenerateRandomPrime(size);
                LoadedPrimeNumber loadp = new LoadedPrimeNumber();
                loadp.PrimeNumber = prime;
                loadp.PrimeSize = size;
                primes.Add(loadp);
            }
            return primes;
        }
    }
}
