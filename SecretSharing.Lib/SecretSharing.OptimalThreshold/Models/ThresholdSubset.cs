#define filterThresholds

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.ProfilerRunner.Models
{
    public class ThresholdSubset
    {
        public static List<String> attemptTrace;
        public static List<String> fixedAttemptTrace;

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
        public static List<ThresholdSubset> findFixedConcatThreshold(IEnumerable<QualifiedSubset> candidateSets, int k, int allparties,int NRSFT)
        {
            int allsentences = candidateSets.Count();

            if (allsentences < 2) return null;

            //fixed element normal case : just a fixed element in front of threshold candidates
            var fixedIntersection = candidateSets.Select(po => po.Parties).Aggregate((first, second) => first.Intersect(second).ToList());

               //add trace 
            fixedAttemptTrace.Add(string.Format("Set:{0}, Attempted fixed item:{1} + threshold:({2},{3})", QualifiedSubset. DumpElementsIntoSingleString(candidateSets)
             , new QualifiedSubset(fixedIntersection).ToString(), k, allparties));
           
               

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
                var smallerQSinvolvedParties = QualifiedSubset.GetAllInvolvedParties(smallerQS).Parties.Count;
                var smallerK = k - fixedIntersection.Count;
                if (nCr(smallerQSinvolvedParties, smallerK) == allsentences)
                {
                    var threshold = findThreshold(smallerQS, smallerK,smallerQSinvolvedParties, NRSFT, false);
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
            }
            return new List<ThresholdSubset>();
        }
        public static List<ThresholdSubset> findThreshold(IEnumerable<QualifiedSubset> candidateSets, int k, int allparties, int NRSFT, bool findIntersection = true)
        {
            int allsentences = candidateSets.Count();
            //add trace 
            attemptTrace.Add(string.Format("Set:{0}, Attempted threshold:({1},{2})", QualifiedSubset.DumpElementsIntoSingleString(candidateSets), k, allparties));

            // normal case
            List<ThresholdSubset> thresholds = new List<ThresholdSubset>();
            var ncr = nCr(allparties, k);
            if (ncr == allsentences)
            {
                ThresholdSubset th = new ThresholdSubset(allparties, k, new List<Trustee>(), QualifiedSubset.GetAllInvolvedParties(candidateSets).Parties);
                thresholds.Add(th);
            }
            /// if the candidate set is larger than nCr then probably some sets don't belong to the threshold 
            /// Here we recusrively rotate parties and select all possible combinations to see which ones are 
            /// building threshold for e.g. p1p2p3 p2p3p4 p1p3p4 p1p2p4 p2p4p5 
            else if (allsentences < ncr)
            {
              
                if (NRSFT > allsentences) NRSFT = GetNumberOfRequiredSetsForThreshold(allparties, k, allsentences);
                if (NRSFT == 1) return thresholds;
              
                //all possible candidate sets must be generated and recursively called
                var smallersetcombinations = candidateSets.Combinations(NRSFT);
                var subsetsAlreadyHaveThreshold = new List<QualifiedSubset>();
                foreach (var smallerset in smallersetcombinations)
                {
                    var newallparties = QualifiedSubset.GetAllInvolvedParties(smallerset).Parties.Count;
                    List<ThresholdSubset> foundthresholds = new List<ThresholdSubset>();
                    //add trace 
                    attemptTrace.Add(string.Format("Set:{0}, Attempted threshold:({1},{2})",
                        QualifiedSubset.DumpElementsIntoSingleString(smallerset), k, newallparties));

                    if (nCr(newallparties, k) == smallerset.Count() && k < newallparties)//not interested in k=n thresholds
                    {
                        foundthresholds = findThreshold(smallerset, k, newallparties, NRSFT, findIntersection);
                        if (foundthresholds.Count > 0)
                        {
                            subsetsAlreadyHaveThreshold.AddRange(smallerset);
                        }
                    }
                    ///try filtering the fixed sets maybe find a threshold 
                    if (foundthresholds.Count == 0 && findIntersection)
                    {
                        foundthresholds = findFixedConcatThreshold(smallerset, k, newallparties, NRSFT);
                    }
                    thresholds.AddRange(foundthresholds);

                }
                if (--NRSFT > 1)
                {
#if filterThresholds
                    var minusAlreadyFoundSets = candidateSets.Except(subsetsAlreadyHaveThreshold);
                    var minusSentences = minusAlreadyFoundSets.Count();
                    if (minusSentences < allsentences)
                    {
                        var minusInvolvedParties = QualifiedSubset.GetAllInvolvedParties(minusAlreadyFoundSets).Parties.Count;
                        //calculate new NRSFT
                        //NRSFT = GetNumberOfRequiredSetsForThreshold(minusInvolvedParties, k, minusAlreadyFoundSets.Count());
                        //if (NRSFT == 1) return thresholds;
                    }
                    thresholds.AddRange(findThreshold(minusAlreadyFoundSets, k, allparties, NRSFT, findIntersection));
#else
                    thresholds.AddRange(findThreshold(candidateSets, k, allparties, NRSFT, findIntersection));
#endif
                }
            }

            return thresholds;
        }

        static int nCr(int n, int r)
        {
            int tmp = 1;
            int j = 2;
            int k = n - r;
            for (int i = n; i > k; i--)
            {
                tmp *= i;
                while (j <= r && tmp % j == 0)
                {
                    tmp /= j++;
                }
            }
            while (j <= r)
            {
                tmp /= j++;
            }
            return tmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allParties"></param>
        /// <param name="numberOfPartiesInTheSubset"></param>
        /// <param name="allSubsets"></param>
        /// <returns></returns>
        public static int GetNumberOfRequiredSetsForThreshold(int allParties, int numberOfPartiesInTheSubset, int allSubsets)
        {

            var a = nCr(allParties, numberOfPartiesInTheSubset);
            if (a > allSubsets)
            {
                return GetNumberOfRequiredSetsForThreshold(--allParties, numberOfPartiesInTheSubset, allSubsets);
            }

            return a;
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
