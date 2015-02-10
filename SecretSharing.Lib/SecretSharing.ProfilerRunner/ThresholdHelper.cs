using SecretSharingCore.Algorithms.GeneralizedAccessStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.ProfilerRunner
{
    public static class ThresholdHelper
    {
        public static IEnumerable<QualifiedSubset> ExploreAllSubsets(ThresholdSubset threshold)
        {
            return threshold.thresholdParties.Combinations(threshold.K).Select(po=> new QualifiedSubset(po));
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        
    }
}
