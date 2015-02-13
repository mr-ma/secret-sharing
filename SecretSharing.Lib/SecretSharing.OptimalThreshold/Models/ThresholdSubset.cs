using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.ProfilerRunner.Models
{
    public class ThresholdSubset
    {
        public static List<String> attemptTrace;

        public IEnumerable<Trustee> fixedParties;
        public IEnumerable<Trustee> thresholdParties;
        int N, K;
        public ThresholdSubset(int N, int K, IEnumerable<Trustee> fixedParties, IEnumerable<Trustee> thresholdParties)
        {
            this.fixedParties = fixedParties;
            this.thresholdParties = thresholdParties;
            this.N = N;
            this.K = K;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ThresholdSubset p = (ThresholdSubset)(obj);
            if (this.fixedParties.Count() == p.fixedParties.Count())
            {

                foreach (var t in p.fixedParties)
                {
                    if (!this.fixedParties.Contains(t))
                        return false;
                }
            }
            else return false;
            if (this.thresholdParties.Count() == p.thresholdParties.Count())
            {

                foreach (var t in p.thresholdParties)
                {
                    if (!this.thresholdParties.Contains(t))
                        return false;
                }
            }
            else return false;
            return true;
        }
        public override int GetHashCode()
        {
            int re = 1;
            foreach (var t in this.fixedParties)
            {
                re ^= t.GetPartyId();
            }
            return re;
        }


        public override String ToString()
        {
            //todo: fix hack tostring from QS
            QualifiedSubset qs = new QualifiedSubset();
            qs.Parties.AddRange(this.thresholdParties);

            QualifiedSubset qss = new QualifiedSubset();
            qss.Parties.AddRange(this.fixedParties);
            if (!String.IsNullOrEmpty(qss.ToString()))
            {
                return String.Format("{0}  Threshold({1},{2})[{3}]", qss.ToString(), K, N, qs.ToString());
            }
            else
            {
                return String.Format("Threshold({0},{1})[{2}]", K, N, qs.ToString());
            }
        }
        public static IEnumerable<QualifiedSubset> ExploreAllSubsets(ThresholdSubset threshold)
        {
            var comb = threshold.thresholdParties.Combinations(threshold.K).Select(po => new QualifiedSubset(po));
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

        #region Threshold detection

        public static IEnumerable<ThresholdSubset> CheckInclusiveThresholds(IEnumerable<ThresholdSubset> thresholds)
        {
            List<ThresholdSubset> alreadyIncludedThresholds = new List<ThresholdSubset>();
            foreach (var th in thresholds)
            {
                var allSubsets = ThresholdSubset.ExploreAllSubsets(th);
                foreach (var sth in thresholds)
                {
                    if (th.Equals(sth))
                        continue;
                    var sallsubs = ThresholdSubset.ExploreAllSubsets(sth);
                    var inclusivethreshold = !sallsubs.Except(allSubsets).Any();
                    if (inclusivethreshold)
                    {
                        alreadyIncludedThresholds.Add(sth);
                    }
                }
            }
            return thresholds.Except(alreadyIncludedThresholds).ToList();
        }
        public static List<ThresholdSubset> findFixedConcatThreshold(IEnumerable<QualifiedSubset> candidateSets, int depth, int allparties,int possibleCombination)
        {
            int allsentences = candidateSets.Count();

            if (allsentences < 2) return null;

            //fixed element normal case : just a fixed element in front of threshold candidates
            var fixedIntersection = candidateSets.Select(po => po.Parties).Aggregate((first, second) => first.Intersect(second).ToList());

               //add trace 
            attemptTrace.Add(string.Format("Set:{0}, Attempted fixed item:{1} + threshold:({2},{3})", QualifiedSubset. DumpElementsIntoSingleString(candidateSets)
             , new QualifiedSubset(fixedIntersection).ToString(), depth, allparties));
           
               

            if (fixedIntersection.Count > 0)
            {
                var smallerQS = new List<QualifiedSubset>();
                //remove fixed part from parties and try to find normal threshold
                foreach (var qs in candidateSets)
                {
                    var withoutFixedParties = qs.Parties.Except(fixedIntersection);
                    if (withoutFixedParties.Count() > 0)
                    {
                        var tempqs = new QualifiedSubset(withoutFixedParties);
                        smallerQS.Add(tempqs);
                    }
                }
                var threshold = findThreshold(smallerQS, depth - fixedIntersection.Count, QualifiedSubset.GetAllInvolvedParties(smallerQS).Parties.Count,possibleCombination,false);
                if (threshold != null)
                //now add the fixed part to the threshold
                {
                    foreach (var th in threshold)
                    {
                        th.fixedParties = fixedIntersection;
                    }
                    
                    return threshold;
                }
            }
            return new List<ThresholdSubset>();
        }

        //public static List<ThresholdSubset> findThreshold(IEnumerable<QualifiedSubset> candidateSets, int depth, int allparties)
        //{
        //    int allsentences = candidateSets.Count();

        //    // normal case
        //    List<ThresholdSubset> thresholds = new List<ThresholdSubset>();
        //    var ncr = nCr(allparties, depth);

        //    //add trace 
        //    attemptTrace.Add(string.Format("Set:{0}, Attempted threshold:({1},{2})", QualifiedSubset.DumpElementsIntoSingleString(candidateSets), depth, allparties));

        //    if (ncr == allsentences)
        //    {
        //        //TODO: check frequencies too
        //        //bool allHaveEqualFrequency = IsAllPartiesFrequencyEqual(candidateSets);
        //        //if (allHaveEqualFrequency)
        //        //{
        //            ThresholdSubset th = new ThresholdSubset(allparties, depth, new List<Trustee>(), QualifiedSubset.GetAllInvolvedParties(candidateSets).Parties);
        //            thresholds.Add(th);
        //        //}
        //    }
        //    /// if the candidate set is larger than nCr then probably some sets don't belong to the threshold 
        //    /// Here we recusrively rotate parties and select all possible combinations to see which ones are 
        //    /// building threshold for e.g. p1p2p3 p2p3p4 p1p3p4 p1p2p4 p2p4p5 
        //    else if (allsentences < ncr)
        //    {
        //        //all possible candidate sets must be generated and recursively called
        //        if (allsentences > 2)
        //        {
        //            var nextPossibleCombination = GetNextPossibleCombiantion(allparties, depth, allsentences);
        //            while (nextPossibleCombination != 1)
        //            {
        //                var smallersetcombinations = candidateSets.Combinations(nextPossibleCombination);
        //                foreach (var smallerset in smallersetcombinations)
        //                {
        //                        var newallparties = QualifiedSubset.GetAllInvolvedParties(smallerset).Parties.Count;
        //                        IEnumerable<ThresholdSubset> foundthresholds;
        //                        if (newallparties == nextPossibleCombination)
        //                        {
        //                            foundthresholds = findThreshold(smallerset, depth, newallparties);
        //                        }
        //                        ///try filtering the fixed sets maybe find a threshold 
        //                        else 
        //                        {
        //                            foundthresholds = findFixedConcatThreshold(smallerset, depth, newallparties);
        //                        }
        //                        if (foundthresholds != null)
        //                        {
        //                            thresholds.AddRange(foundthresholds);
        //                        }
        //                }
        //                nextPossibleCombination --;
        //            }

        //        }
        //    }

        //    return thresholds;
        //}





        public static List<ThresholdSubset> findThreshold(IEnumerable<QualifiedSubset> candidateSets, int depth, int allparties,int possibleCombination, bool findIntersection = true)
        {
            int allsentences = candidateSets.Count();
            //add trace 
            attemptTrace.Add(string.Format("Set:{0}, Attempted threshold:({1},{2})", QualifiedSubset.DumpElementsIntoSingleString(candidateSets), depth, allparties));

            // normal case
            List<ThresholdSubset> thresholds = new List<ThresholdSubset>();
            var ncr = nCr(allparties, depth);
            if (ncr == allsentences)
            {
                ThresholdSubset th = new ThresholdSubset(allparties, depth, new List<Trustee>(), QualifiedSubset.GetAllInvolvedParties(candidateSets).Parties);
                thresholds.Add(th);
            }
            /// if the candidate set is larger than nCr then probably some sets don't belong to the threshold 
            /// Here we recusrively rotate parties and select all possible combinations to see which ones are 
            /// building threshold for e.g. p1p2p3 p2p3p4 p1p3p4 p1p2p4 p2p4p5 
            else if (allsentences < ncr)
            {
                //all possible candidate sets must be generated and recursively called
                if (allsentences > 2)
                {
                    var smallersetcombinations = candidateSets.Combinations(possibleCombination);
                    foreach (var smallerset in smallersetcombinations)
                    {
                        var newallparties = QualifiedSubset.GetAllInvolvedParties(smallerset).Parties.Count;
                        List<ThresholdSubset> foundthresholds=new List<ThresholdSubset>();
                        if (newallparties == possibleCombination)
                        {
                            foundthresholds = findThreshold(smallerset, depth, newallparties, possibleCombination,findIntersection);
                        }
                        ///try filtering the fixed sets maybe find a threshold 
                        else if(findIntersection)
                        {
                            foundthresholds = findFixedConcatThreshold(smallerset, depth, newallparties, possibleCombination);
                        }
                        thresholds.AddRange(foundthresholds);
                    }
                    if (--possibleCombination != 1)
                        thresholds.AddRange(findThreshold(candidateSets, depth, allparties, possibleCombination,findIntersection));
                }

         
            }

            return thresholds;
        }

        static int nCr(int m, int n)
        {
            int tmp = 1;
            int j = 2;
            int k = m - n;
            for (int i = m; i > k; i--)
            {
                tmp *= i;
                while (j <= n && tmp % j == 0)
                {
                    tmp /= j++;
                }
            }
            while (j <= n)
            {
                tmp /= j++;
            }
            return tmp;
        }

        public static int GetNextPossibleCombiantion(int allParties, int depth, int allsentences)
        {

            var a = nCr(allParties, depth);
            while (a >= allsentences)
                a = nCr(--allParties, depth);

            return allParties;
        }
        static bool IsAllPartiesFrequencyEqual(IEnumerable<QualifiedSubset> candidateSets)
        {
            var frequencie = new List<Tuple<int, int>>();
            foreach (QualifiedSubset qs in candidateSets)
            {
                frequencie.AddRange(qs.Parties.GroupBy(po => po.partyId).Select(grp => new Tuple<int, int>(grp.Key, grp.Count())));
            }

            var itemsFrequencies = frequencie.GroupBy(po => po.Item1).Select(group => new Tuple<int, int>(group.Key, group.Sum(grp => grp.Item2)));
            bool allHaveEqualFrequency = false;
            if (itemsFrequencies.Count() > 0)
            {
                var firstItem = itemsFrequencies.First();
                allHaveEqualFrequency = itemsFrequencies.All(s => s.Item2 == firstItem.Item2);
            }
            return allHaveEqualFrequency;
        }

        #endregion

    }
}
