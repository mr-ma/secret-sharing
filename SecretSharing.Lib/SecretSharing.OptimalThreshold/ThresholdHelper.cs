
using SecretSharing.ProfilerRunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.ProfilerRunner
{
    public static class ThresholdHelper
    {
       
        public static void ServeThresholdDetection(AccessStructure access, out List<QualifiedSubset> expandedSet
           , out  List<QualifiedSubset> qualifiedSet, out List<ThresholdSubset> thresholds, out List<string> attempts,
           out List<QualifiedSubset> notMatchingSet)
        {
            ThresholdSubset.attemptTrace = new List<string>();

            List<Trustee> trustees = access.GetAllParties().OrderBy(po => po.partyId).ToList();

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = AccessStructure.ExpandPersonPaths(null, trustees).Distinct().ToList();
            expandedSet = subsets;

            //return;

            //we don't care about longer paths which are not mentioned in the access structure
            int longestQualifiedSubsetAccepted = access.GetLongestLength();//GetLongestLength(access);

            //calculate the share of each party in qualified subsets
            int i = 0;
            List<Tuple<Trustee, int>> allsubsetsparties = new List<Tuple<Trustee, int>>();
            List<QualifiedSubset> qualifiedExpandedSubset = new List<QualifiedSubset>();
            Console.WriteLine("All qualified expanded subsets:");
            foreach (QualifiedSubset subset in subsets)
            {
                //delete unqualified subsets based on minimum access structure
                if (subset.Parties.Count > longestQualifiedSubsetAccepted) continue;
                bool isqualified = AccessStructure.IsQualifiedSubset(subset, access);
                if (isqualified)
                {
                    //add the subset to expanded qualified
                    qualifiedExpandedSubset.Add(subset);
                    allsubsetsparties.AddRange(subset.Parties.Select(po => new Tuple<Trustee, int>(po, subset.Parties.Count)));
                    Console.WriteLine("{0}.\t [{1}] q:{2}", i++, subset.ToString(), isqualified);
                }
            }

            qualifiedSet = qualifiedExpandedSubset.ToList();



            List<ThresholdSubset> thresholdsubsets = new List<ThresholdSubset>();
            var normalTH = qualifiedExpandedSubset.GroupBy(po => po.Parties.Count).Select(info => new { Depth = info.Key, Count = info.Count() });
            foreach (var th in normalTH)
            {
                var candidateSets = qualifiedExpandedSubset.Where(po => po.Parties.Count == th.Depth);

                ///find threshold in sets
                var threshold = ThresholdSubset.findThreshold(candidateSets, th.Depth, trustees.Count,
                    ThresholdSubset.GetNextPossibleCombiantion(trustees.Count,th.Depth,candidateSets.Count()));
                //if (threshold == null || threshold.Count == 0)
                //{
                //    //maybe it has a fixed item let's try with fixed item threshold detector
                //    threshold = ThresholdSubset.findFixedConcatThreshold(candidateSets, th.Depth, trustees.Count);
                //}

                if (threshold != null && threshold.Count != 0)
                {
                    thresholdsubsets.AddRange(threshold);
                }

            }


            /*foreach (ThresholdSubset th in ThresholdSubset.CheckInclusiveThresholds(thresholdsubsets.Distinct()))
            {
                Console.WriteLine(th);
                var coveredsets = ThresholdSubset.ExploreAllSubsets(th);
                // Console.WriteLine("covered sets:");
                //DumpElements(coveredsets);

                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }*/
            thresholds = thresholdsubsets.Distinct().ToList();
            notMatchingSet = qualifiedExpandedSubset.ToList();

            attempts = ThresholdSubset.attemptTrace;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
      
        
    }
}
