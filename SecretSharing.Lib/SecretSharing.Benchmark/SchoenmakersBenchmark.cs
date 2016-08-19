using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecretSharingCore.Common;
using SecretSharingCore.Algorithms.PVSS;
using System.Diagnostics;
using SecretSharingCore.Algorithms.PKE;
using SecretSharingCore;

namespace SecretSharing.Benchmark
{
    public class SchoenmakersBenchmark//: IBenchmark
    {
        PublicKeyEncryption pke;
        List<Tuple<byte[], byte[]>> keypairs;

        public List<SecretSharingBenchmarkReport> BenchmarkAllKeys( int MinN, int MaxN, int MinK, int MaxK, string[] Keys,
            int step, SecretSharingBenchmarkReport.OperationType operation, int iterate, List<LoadedPrimeNumber> loadedPrimes)
        {
            var result = new List<SecretSharingBenchmarkReport>();
            foreach (var key in Keys)
            {
                result.AddRange(BenchmarkKey(MinN, MaxN, MinK, MaxK, step, operation, key, iterate,loadedPrimes));
            }
            return result;
        }
        public IEnumerable<SecretSharingBenchmarkReport> BenchmarkKey(int MinN, int MaxN, int MinK, int MaxK, int step,
            SecretSharingBenchmarkReport.OperationType operation, string key, int iterate, List<LoadedPrimeNumber> loadedPrimes)
        {
            List<SecretSharingBenchmarkReport> results = new List<SecretSharingBenchmarkReport>();
            var filteredPrimes = loadedPrimes.Where(po => po.PrimeSize == key.Length*8).ToList();
            PrimeGenerator.SetLoadedPrimes(filteredPrimes);

            Schoenmakers schoenmakers = new Schoenmakers();
            pke = new PublicKeyEncryption();

            //TODO: ensure it's secure enough
            int fieldSize = key.Length * 8;

            for (int n = MinN; n <= MaxN; n += step)
            {
                //k can not be bigger than n

                //Parallel.For(1, MaxK, k =>
                //{
                List<long> initProtocolElapsedTicks = new List<long>();
                Antix.Testing.Benchmark.Run(() => InitProtocolWrapper(ref schoenmakers, n, fieldSize, ref initProtocolElapsedTicks), iterate);
                results.Add(new SecretSharingBenchmarkReport()
                                           {
                                               n = n,
                                               k = 0,
                                               ElapsedTicks = initProtocolElapsedTicks.ToArray(),
                                               keyLength = key.Length * 8,
                                               Operation = SecretSharingBenchmarkReport.OperationType.InitProtocol,
                                           });
                //for (int k = MinK; k <= n && k <= MaxK; )
                //{
                //            List<long> verifySharesElapsedTicks = new List<long>();
                            
                //            List<long> divideRandomSecretElapsedTicks = new List<long>();
                //            var decryptShareElapsedTicks = new List<long>();
                //            var verifyDecryptedSharesTicks = new List<long>();
                //            var reconstructRandomSecretElapsedTicks = new List<long>();
                //            byte[] secret = null;
                //            Antix.Testing.Benchmark.Run(() =>
                //            {
                //                List<Byte[]> Commitments = new List<byte[]>();
                //                byte[] U = null;
                //                List<SchoenmakersShare> shares = null;
                                
                //                shares = DivideRandomSecretWrapper(ref schoenmakers, n, k, ref secret, ref divideRandomSecretElapsedTicks, ref Commitments);
                //                List<byte[]> publickeys = keypairs.Select(po => po.Item2).ToList();
                //                VerifyDistributedShares(ref schoenmakers, shares, Commitments, publickeys, ref verifySharesElapsedTicks);
                //                DecryptSharesWrapper(ref schoenmakers, ref shares, keypairs, ref decryptShareElapsedTicks);
                //                VerifyDecryptedShares(ref schoenmakers, shares, k, ref verifyDecryptedSharesTicks);
                //                ReconstructRandomSecretWrapper(ref schoenmakers, shares, k, ref reconstructRandomSecretElapsedTicks);
                //            }, iterate);

                //            #region adding results

                //            results.Add(new SecretSharingBenchmarkReport()
                //            {
                //                n = n,
                //                k = k,
                //                ElapsedTicks = divideRandomSecretElapsedTicks.ToArray(),
                //                keyLength = key.Length * 8,
                //                Operation = SecretSharingBenchmarkReport.OperationType.ShareGenerationRandomSecret,
                //            });

                //            results.Add(new SecretSharingBenchmarkReport()
                //            {
                //                n = n,
                //                k = k,
                //                ElapsedTicks = verifySharesElapsedTicks.ToArray(),
                //                keyLength = key.Length * 8,
                //                Operation = SecretSharingBenchmarkReport.OperationType.VerifyShares,
                //            });
                                 
                //            results.Add(new SecretSharingBenchmarkReport()
                //            {
                //                n = n,
                //                k = k,
                //                ElapsedTicks = decryptShareElapsedTicks.ToArray(),
                //                keyLength = key.Length * 8,
                //                Operation = SecretSharingBenchmarkReport.OperationType.DecryptShares,
                //            });

                             
                //            results.Add(new SecretSharingBenchmarkReport()
                //            {
                //                n = n,
                //                k = k,
                //                ElapsedTicks = verifyDecryptedSharesTicks.ToArray(),
                //                keyLength = key.Length * 8,
                //                Operation = SecretSharingBenchmarkReport.OperationType.VerifyPooledShares,
                //            });

                           
                //            results.Add(new SecretSharingBenchmarkReport()
                //            {
                //                n = n,
                //                k = k,
                //                ElapsedTicks = reconstructRandomSecretElapsedTicks.ToArray(),
                //                keyLength = key.Length * 8,
                //                Operation = SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction,
                //            });
                //            #endregion

                //            Console.WriteLine("Iteration info: n:{0} t:{1} keySize:{2}", n, k, key/*s[i]*/.Length * 8);
                //    if (k == 1) k = step;
                //    else k += step;
                //}
            }//);
            var orderedResults = results.OrderBy(po => po.n).ThenBy(po => po.k);
            return orderedResults;
        }
        public List<SchoenmakersShare> DivideSpecialSecretWrapper(ref Schoenmakers schoenmakers,int n, int t, string Secret, ref List<long> ElapsedTicks, ref List<byte[]> commitments, ref byte[] U)
        {
            var secretBytes = Encoding.UTF8.GetBytes(Secret);
            byte[] randomsecret = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var shares = schoenmakers.Distribute(t, n, secretBytes, ref commitments, ref randomsecret,ref U);
            sw.Stop();

            ElapsedTicks.Add(sw.ElapsedTicks);
            return shares;
        }
        public List<SchoenmakersShare> DivideRandomSecretWrapper(ref Schoenmakers schoenmakers, int n, int t,ref byte[] randomsecret, ref List<long> ElapsedTicks, ref List<byte[]> commitments)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var shares = schoenmakers.Distribute(t, n, ref commitments, ref randomsecret);
            sw.Stop();

            ElapsedTicks.Add(sw.ElapsedTicks);
            return shares;
        }
        public void InitProtocolWrapper(ref Schoenmakers schoenmakers, int n, int fieldSizeByte,ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            schoenmakers.SelectPrimeAndGenerators(fieldSizeByte);
            keypairs = new List<Tuple<byte[], byte[]>>();
            for (int i = 0; i < n; i++)
            {
                var pair = pke.GenerateKeyPair(schoenmakers.GetqB(), schoenmakers.GetGB());
                keypairs.Add(pair);
            }
            schoenmakers.SetPublicKeys(keypairs.Select(po => po.Item2).ToList());
            sw.Stop();
            elapsedTicks.Add(sw.ElapsedTicks);
        }


        public void ReconstructRandomSecretWrapper(ref Schoenmakers schoenmakers, List<SchoenmakersShare> shares, int t, ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            schoenmakers.Reconstruct(t, shares);
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }
        public void ReconstructSpecialSecretWrapper(ref Schoenmakers schoenmakers, List<SchoenmakersShare> shares, int t,byte[] U, ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            schoenmakers.Reconstruct(t, shares,U);
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }
        public void DecryptSharesWrapper(ref Schoenmakers schoenmakers,ref List<SchoenmakersShare> shares, List<Tuple<byte[], byte[]>> keypairs, ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < shares.Count; i++)
            {
                var share = shares[i];
                schoenmakers.PoolShare(keypairs[i], ref share);
                shares[i] = share;
            }
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }
        public void VerifyDistributedShares(ref Schoenmakers schoenmakers, List<SchoenmakersShare> shares, 
            List<Byte[]> Commitments, List<byte[]> publicKeys, ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < shares.Count; i++)
            {
                bool verifiedShare = schoenmakers.VerifyDistributedShare(i + 1, shares[i], Commitments, keypairs[i].Item2);
            }
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }

        public void VerifyDecryptedShares(ref Schoenmakers schoenmakers, List<SchoenmakersShare> decryptedShares,
        int t, ref List<long> elapsedTicks)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool verifiedShare = schoenmakers.VerifyPooledShares(t, decryptedShares);
            sw.Stop();

            elapsedTicks.Add(sw.ElapsedTicks);
        }
    }
}
