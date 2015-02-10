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
            var comb = threshold.thresholdParties.Combinations(threshold.K).Select(po=> new QualifiedSubset(po));
            if (threshold.fixedParties != null && threshold.fixedParties.Count() > 0)
            {
                List<QualifiedSubset> largerQS = new List<QualifiedSubset>();
                foreach (var qs in comb)
                {
                    QualifiedSubset lqs = new QualifiedSubset(threshold.fixedParties.Union(qs.Parties));
                    largerQS.Add(lqs);
                }
                return largerQS;
            }
            return comb;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        
    }
}
