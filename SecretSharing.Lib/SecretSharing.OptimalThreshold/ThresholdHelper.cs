
using SecretSharing.OptimalThreshold.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.OptimalThreshold
{
    public static class ThresholdHelper
    {
       
        public static void ServeThresholdDetection(AccessStructure access, bool tryIntersect, out List<QualifiedSubset> expandedSet
           , out  List<QualifiedSubset> qualifiedSet, out List<ThresholdSubset> thresholds, out List<string> attempts,
           out List<QualifiedSubset> notMatchingSet)
        {
            ThresholdSubset.attemptTrace = new List<string>();
            ThresholdSubset.fixedAttemptTrace = new List<string>();

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
                    ThresholdSubset.GetNumberOfRequiredSetsForThreshold(trustees.Count,th.Depth,candidateSets.Count()),tryIntersect);
                if (threshold != null && threshold.Count != 0)
                {
                    thresholdsubsets.AddRange(threshold);
                }

            }

            var uniqueInclusiveThresholds = ThresholdSubset.CheckInclusiveThresholds(thresholdsubsets.Distinct());
            foreach (ThresholdSubset th in uniqueInclusiveThresholds)
            {
                Console.WriteLine(th);
                var coveredsets = ThresholdSubset.ExploreAllSubsets(th);
                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }
            thresholds = uniqueInclusiveThresholds.ToList();
            notMatchingSet = qualifiedExpandedSubset.ToList();

            attempts = ThresholdSubset.attemptTrace;
        }

        public static void DetectThresholds(AccessStructure access,bool tryIntersect, out List<ThresholdSubset> thresholds, out List<QualifiedSubset> notMatchingSet)
        {
            var expanded = new List<QualifiedSubset>();
            var qualified = new List<QualifiedSubset>();
            var attempts = new List<String>();
            ThresholdHelper.ServeThresholdDetection(access,tryIntersect, out expanded, out  qualified, out  thresholds, out attempts, out notMatchingSet);
        }


        public static AccessStructure OptimiseAccessStructure(AccessStructure access,bool tryIntersect)
        {
            var thresholds = new List<ThresholdSubset>();
            var remaining = new List<QualifiedSubset>();
            DetectThresholds(access, tryIntersect, out thresholds, out remaining);
            if(IsThresholdShareShorter(access,thresholds,remaining)){
                var optimisedAccess = new AccessStructure();
                optimisedAccess.Accesses.AddRange(thresholds);
                optimisedAccess.Accesses.AddRange(remaining);
                return optimisedAccess;
            }
            return access;
        }

        public static bool IsThresholdShareShorter(AccessStructure access, List<ThresholdSubset> thresholds
            , List<QualifiedSubset> notMatchingSet)
        {
            // caclulate shares given in case of using thresholds
            var sumThresholdShare = 0;
            foreach (var th in thresholds)
            {
                sumThresholdShare += th.getPartiesCount();
            }
            //append shares from the not matching sets
            foreach (var qs in notMatchingSet)
            {
                sumThresholdShare += qs.Parties.Count;
            }
            var sumNoThreshold = 0;
            //now let's see how it would be if we just divide simply
            foreach (var qs in access.Accesses)
            {
                sumNoThreshold += qs.getPartiesCount();
            }
            return sumThresholdShare < sumNoThreshold;
        }

 

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
      
        
    }
}
