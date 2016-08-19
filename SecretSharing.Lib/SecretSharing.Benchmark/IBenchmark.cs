using System;
namespace SecretSharing.Benchmark
{
    interface IBenchmark
    {
        System.Collections.Generic.List<SecretSharingBenchmarkReport> BenchmarkAllKeysWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK, int MaxK, string[] Keys, int step, SecretSharingBenchmarkReport.OperationType operation, int iterate);
        System.Collections.Generic.IEnumerable<SecretSharingBenchmarkReport> BenchmarkPrimeWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK, int MaxK, int step, string key, int iterate);
        System.Collections.Generic.List<SecretSharingBenchmarkReport> BenchmarkPrimeWithChunkSize(int[] chunkSize, int MinN, int MaxN, int MinK, int MaxK, string[] Keys, int step, SecretSharingBenchmarkReport.OperationType operation, int iterate);
        System.Collections.Generic.List<SecretSharingCore.Common.IShareCollection> DivideSecretWithChunkSizeWrapper(int n, int k, int ChunkSize, string Secret, ref System.Collections.Generic.List<long> ElapsedTicks);
        void GeneratePrimeWithChunkSizeWrapper(int ChunkSize, ref System.Collections.Generic.List<long> elapsedTicks);
        void ReconstructSecretWithChunkSizeWrapper(System.Collections.Generic.List<SecretSharingCore.Common.IShareCollection> shares, int k, int chunkSize, ref System.Collections.Generic.List<long> elapsedTicks);
    }
}
