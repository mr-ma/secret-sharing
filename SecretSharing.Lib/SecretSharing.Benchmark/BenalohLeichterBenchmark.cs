using SecretSharing.OptimalThreshold;
using SecretSharing.OptimalThreshold.Models;
using SecretSharingCore;
using SecretSharingCore.Algorithms.GeneralizedAccessStructure;
using SecretSharingCore.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SecretSharing.Benchmark
{
    public enum OperationType
    {
        DivideSecretOptimisedIntersected=0,
        DivideSecretOptimised=1,
        DivideSecret=2,
        ReconstructSecret=3,
    }
    [Serializable]
    public class BenalohLeichterBenchmarkReport
    {
        public int NumberOfShares;
        public long[] ElapsedTicks { get; set; }
        public OperationType Operation;
        public int KeyLength;
        public String Access;
        public String OptimisedAccess;

    }
    public class BenalohLeichterBenchmark
    {
        static IEnumerable<IEnumerable<T>>
    GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static List<AccessStructure> ExploreAllPossibleAccessStructures(IEnumerable<Trustee> participants)
        {
            List<QualifiedSubset> qss = new List<QualifiedSubset>();
            // build up all possible access structure for given input trustees
            for (int i = 1; i <= participants.Count(); i++)
            {
                var combs = GetKCombs<Trustee>(participants, i);
                foreach (var com in combs)
                {
                    QualifiedSubset qs = new QualifiedSubset(com);
                    qss.Add(qs);
                    Console.Write(qs.ToString()+'\t');
                }
                Console.WriteLine();
            }

            List<AccessStructure> accesses = new List<AccessStructure>();
            for (int i = 2; i <= qss.Count; i++)
            {
               
               var combinedaccesses =   GetKCombs<QualifiedSubset>(qss, i);
               foreach (var ac in combinedaccesses)
               {
                   var access = new AccessStructure(ac.ToList());
                   accesses.Add(access);
                   Console.Write(access.ToString()+'\t');
               }

               Console.WriteLine();
            }

            return accesses;
        }

        public List<BenalohLeichterBenchmarkReportSet> BenchmarkAllAccesses(IEnumerable<Trustee> participants, string key, int iterate, List<AccessStructure> accesses, string type, List<LoadedPrimeNumber> primes)
        {
            var randomacc = accesses;
            //when benchmarking share length pick 1000 random elements from the access structure
            if (type == "share")
            {
                randomacc = accesses.PickRandom(1000).ToList();
            }
            var results = new List<BenalohLeichterBenchmarkReportSet>();
            foreach (var access in randomacc)
            {
                switch (type)
                {
                    case "scheme":
                        results.AddRange(benchmarkScheme(access, key, iterate, primes));
                        break;
                    case "share":
                        results.AddRange(benchmarkShareLengthAccessStructure(access, key, iterate));
                        break;
                }

            }
            return results;
        }

        private IEnumerable<BenalohLeichterBenchmarkReportSet> benchmarkShareLengthAccessStructure(AccessStructure access, string key,int iterate)
        {
            List<BenalohLeichterBenchmarkReportSet> results = new List<BenalohLeichterBenchmarkReportSet>();
            List<long> elapsedDivideOptimisdIntersected = new List<long>();
            List<long> elapsedDivideOptimisd = new List<long>();
            List<long> elapsedDivide = new List<long>();
            List<long> elapsedReconstruction = new List<long>();
            int shares = 0;
            AccessStructure optimsedAccess = null;
            Antix.Testing.Benchmark.Run(() =>
            {
                shares = wrappedShareLengthDivideSecret(access, true, true, key, ref elapsedDivideOptimisdIntersected, ref optimsedAccess);
            }, iterate);

            var numberOfShares = shares; 
            BenalohLeichterBenchmarkReport divideReportOptimisedIntersected = new BenalohLeichterBenchmarkReport()
            {
                Access = access.ToString(),
                ElapsedTicks = elapsedDivideOptimisdIntersected.ToArray(),
                KeyLength = key.Length * 8,
                NumberOfShares = numberOfShares,
                Operation = OperationType.DivideSecretOptimisedIntersected,
                OptimisedAccess = optimsedAccess.ToString(),
            };

            Antix.Testing.Benchmark.Run(() =>
            {
                shares = wrappedShareLengthDivideSecret(access, true, false, key, ref elapsedDivideOptimisd, ref optimsedAccess);
            }, iterate);
            numberOfShares = shares;
            BenalohLeichterBenchmarkReport divideReportOptimised = new BenalohLeichterBenchmarkReport()
            {
                Access = access.ToString(),
                ElapsedTicks = elapsedDivideOptimisd.ToArray(),
                KeyLength = key.Length * 8,
                NumberOfShares = numberOfShares,
                Operation = OperationType.DivideSecretOptimised,
                OptimisedAccess = optimsedAccess.ToString(),

            };

            Antix.Testing.Benchmark.Run(() =>
            {
                shares = wrappedShareLengthDivideSecret(access, false, false, key, ref elapsedDivide, ref optimsedAccess);
            }, iterate);
            numberOfShares = shares;
            BenalohLeichterBenchmarkReport divideReport = new BenalohLeichterBenchmarkReport()
            {
                Access = access.ToString(),
                ElapsedTicks = elapsedDivide.ToArray(),
                KeyLength = key.Length * 8,
                NumberOfShares = numberOfShares,
                Operation = OperationType.DivideSecret
            };

            var setReport = new BenalohLeichterBenchmarkReportSet()
            {
                divideItem = divideReport,
                optimisedItem = divideReportOptimised,
                IntersectedItem = divideReportOptimisedIntersected
            };
            results.Add(setReport);
 

            return results;
        }

        private IEnumerable<BenalohLeichterBenchmarkReportSet> benchmarkScheme(AccessStructure access, string key, int iterate,List<LoadedPrimeNumber> primes)
        {
            Console.WriteLine("benchmarking {0}...",access);
            var filteredPrimes = primes.Where(po => po.PrimeSize == key.Length).ToList();
            PrimeGenerator.SetLoadedPrimes(filteredPrimes);
            List<BenalohLeichterBenchmarkReportSet> results = new List<BenalohLeichterBenchmarkReportSet>();
            List<long> elapsedDivide = new List<long>();
            List<long> elapsedReconstruction = new List<long>();
            List<IShareCollection> shares =null;

            Antix.Testing.Benchmark.Run(() =>
            {
                shares = wrappedDivideSecret(access, key, ref elapsedDivide);
            }, iterate);
            BenalohLeichterBenchmarkReport divideReport = new BenalohLeichterBenchmarkReport()
            {
                Access = access.ToString(),
                ElapsedTicks = elapsedDivide.ToArray(),
                KeyLength = key.Length * 8,
                Operation = OperationType.DivideSecret
            };

            Antix.Testing.Benchmark.Run(() =>
            {
                wrappedConstructSecret(shares, ref elapsedReconstruction);
            }, iterate);

            BenalohLeichterBenchmarkReport reconstructReport = new BenalohLeichterBenchmarkReport()
            {
                Access = access.ToString(),
                ElapsedTicks = elapsedReconstruction.ToArray(),
                KeyLength = key.Length * 8,
                Operation = OperationType.ReconstructSecret
            };
            var setReport = new BenalohLeichterBenchmarkReportSet()
            {
                divideItem = divideReport,
                reconstructItem = reconstructReport,
            };
            results.Add(setReport);
            return results;
        }
        private int getNumberOfShares(List<IShareCollection> shares)
        {
            var sum = 0;
            foreach (var item in shares)
            {
                sum += item.GetCount();
            }
            return sum;
        }
        private int wrappedShareLengthDivideSecret(AccessStructure access,bool optimise,bool tryIntersect,string secret, ref List<Int64> elapsedTicks, ref AccessStructure optimisedAccess)
        {
            //return new List<IShareCollection>();

            optimisedAccess = null;
            var benaloh = new BenalohLeichter();
            var sw = new Stopwatch();
            var secretbytes = Encoding.UTF8.GetBytes(secret.ToCharArray());
            sw.Start();
            if (optimise)
            {
                access = ThresholdHelper.OptimiseAccessStructure(access, tryIntersect);
                optimisedAccess = access;
            }
            var shares = getMockShares(access); //benaloh.DivideSecret(secretbytes, access);
            sw.Stop();
            
            elapsedTicks.Add(sw.ElapsedTicks);

            return shares;
        }
        private List<IShareCollection> wrappedDivideSecret(AccessStructure access, string secret, ref List<Int64> elapsedTicks)
        {
            var benaloh = new BenalohLeichter();
            var sw = new Stopwatch();
            var secretbytes = Encoding.UTF8.GetBytes(secret.ToCharArray());
            sw.Start();
            var shares = benaloh.DivideSecret(secretbytes, access);
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);

            return shares;
        }

        private int getMockShares(AccessStructure access)
        {
            var sum = 0;
            foreach (var qs in access.Accesses)
            {
                sum += qs.getShareBranchesCount();
            }
            return sum;
        }
        private void  wrappedConstructSecret(List<IShareCollection> shares, ref List<Int64> elapsedTicks)
        {
            var benaloh = new BenalohLeichter();
            var sw = new Stopwatch();
            sw.Start();
            foreach (IShareCollection share in shares)
            {
                benaloh.ReconstructSecret(share);
            }
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }
    }
}
