using pUnit;
using SecretSharingCore.Algorithms.GeneralizedAccessStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.ProfilerRunner
{
    public static class Program
    {

       public static void ServeThresholdDetection(AccessStructure access, out List<QualifiedSubset> expandedSet
            , out  List<QualifiedSubset> qualifiedSet, out List<ThresholdSubset> thresholds, 
            out List<QualifiedSubset> notMatchingSet)
        {
            List<Trustee> trustees = access.GetAllParties().OrderBy(po => po.partyId).ToList();

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = ExpandPersonPaths(null, trustees).Distinct().ToList();
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
                bool isqualified = IsQualifiedSubset(subset, access);
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
                var threshold = findThreshold(candidateSets, th.Depth, trustees.Count);
                if (threshold.Count == 0)
                {
                    //maybe it has a fixed item let's try with fixed item threshold detector
                    threshold = findFixedConcatThreshold(candidateSets, th.Depth, trustees.Count);
                }

                if (threshold != null && threshold.Count != 0)
                {
                    thresholdsubsets.AddRange(threshold);
                }

            }


            foreach (ThresholdSubset th in CheckInclusiveThresholds(thresholdsubsets.Distinct()))
            {
                Console.WriteLine(th);
                var coveredsets = ThresholdHelper.ExploreAllSubsets(th);
                // Console.WriteLine("covered sets:");
                //DumpElements(coveredsets);

                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }
            thresholds = thresholdsubsets;
            notMatchingSet = qualifiedExpandedSubset.ToList();
        }

        static void Main(string[] args)
        {
            
           // permutation(3, "1234");
           // Console.Read();
           // Console.WriteLine(produceList()) ;
            // qualified subsets for minimal access structure
            //P2^P3,P1^P2^P4,P1^P3^P4
            //P1^P3^P4,P1^P2,P2^P3
            //P1^P3^P4,P1^P2,P2^P3,P2^P4
            //P1^P2^P3,P1^P2^P4,P1^P3^P4
            //no threshold: P1^P2,P2^P3,P3^p4,p4^p5,p5^p6,p6^p7,p7^p8,p8^p1
            //threshold ruiner: p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7

            AccessStructure access = new AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7");
            List<Trustee> trustees = access.GetAllParties().OrderBy(po=>po.partyId).ToList();

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = ExpandPersonPaths(null, trustees).Distinct().ToList();
            Console.WriteLine("All expanded subsets:");
            DumpElements(subsets);
           
            //return;

            //we don't care about longer paths which are not mentioned in the access structure
            int longestQualifiedSubsetAccepted = access.GetLongestLength();//GetLongestLength(access);

            //calculate the share of each party in qualified subsets
            int i = 0;
            List<Tuple< Trustee,int>> allsubsetsparties = new List<Tuple< Trustee,int>>();
            List<QualifiedSubset> qualifiedExpandedSubset = new List<QualifiedSubset>();
            Console.WriteLine("All qualified expanded subsets:");
            foreach (QualifiedSubset subset in subsets)
            {
                //delete unqualified subsets based on minimum access structure
                if (subset.Parties.Count > longestQualifiedSubsetAccepted) continue;
                bool isqualified = IsQualifiedSubset(subset, access);
                if (isqualified)
                {
                    //add the subset to expanded qualified
                    qualifiedExpandedSubset.Add(subset);
                    allsubsetsparties.AddRange(subset.Parties.Select(po => new Tuple<Trustee, int>(po, subset.Parties.Count)));
                    Console.WriteLine("{0}.\t [{1}] q:{2}", i++, subset.ToString(), isqualified);
                }
            }





            List<ThresholdSubset> thresholdsubsets = new List<ThresholdSubset>();
            var normalTH = qualifiedExpandedSubset.GroupBy(po => po.Parties.Count).Select(info => new {Depth = info.Key,Count= info.Count()});
            foreach (var th in normalTH)
            {
               var candidateSets = qualifiedExpandedSubset.Where(po => po.Parties.Count == th.Depth);
               
                ///find threshold in sets
               var threshold = findThreshold(candidateSets,th.Depth,trustees.Count);
               if (threshold.Count == 0) 
               {  
                   //maybe it has a fixed item let's try with fixed item threshold detector
                   threshold = findFixedConcatThreshold(candidateSets, th.Depth, trustees.Count); 
               }

               if ( threshold!=null && threshold.Count != 0) 
               { 
                   thresholdsubsets.AddRange(threshold);
               }
               
            }

            Console.WriteLine("All detected optimal thresholds:");
            foreach (ThresholdSubset th in CheckInclusiveThresholds(thresholdsubsets.Distinct()))
            {
                Console.WriteLine(th);
                var coveredsets =ThresholdHelper.ExploreAllSubsets( th);
               // Console.WriteLine("covered sets:");
                //DumpElements(coveredsets);

                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }

            Console.WriteLine("All remaining subsets:");
            DumpElements(qualifiedExpandedSubset);

            //thresholdsubsets + qualifiedExpandedSubset is the share;
            Console.ReadLine();
            return;
        }

        public  static IEnumerable<ThresholdSubset> CheckInclusiveThresholds(IEnumerable<ThresholdSubset> thresholds)
        {
            List<ThresholdSubset> alreadyIncludedThresholds = new List<ThresholdSubset>();
            foreach (var th in thresholds)
            {
                var allSubsets = ThresholdHelper.ExploreAllSubsets(th);
                foreach (var sth in thresholds)
                {
                    if (th.Equals(sth)) 
                        continue;
                    var sallsubs = ThresholdHelper.ExploreAllSubsets(sth);
                    var inclusivethreshold = !sallsubs.Except(allSubsets).Any();
                    if (inclusivethreshold)
                    {
                        alreadyIncludedThresholds.Add(sth);
                    }
                }
            }  
            return thresholds.Except(alreadyIncludedThresholds).ToList();
        }

        public static QualifiedSubset GetAllInvolvedParties(IEnumerable<QualifiedSubset> elements)
        {
            var sum = new QualifiedSubset();
            foreach (var qs in elements)
            {
                foreach (var party in qs.Parties)
                {
                    if (!sum.Parties.Contains(party))
                    {
                        sum.Parties.Add(party);
                    }
                }
            }

            return sum;
        }

        static List<ThresholdSubset> findFixedConcatThreshold(IEnumerable<QualifiedSubset> candidateSets, int depth, int allparties)
        {
            //fixed element normal case : just a fixed element in front of threshold candidates
            var fixedIntersection = candidateSets.Select(po => po.Parties).Aggregate((first, second) => first.Intersect(second).ToList());

            if (fixedIntersection.Count > 0)
            {
                var smallerQS = new List<QualifiedSubset>();
                //remove fixed part from parties and try to find normal threshold
                foreach (var qs in candidateSets)
                {
                    var tempqs = new QualifiedSubset( qs.Parties.Except(fixedIntersection));
                    smallerQS.Add(tempqs);
                }
                var threshold = findThreshold(smallerQS, depth - fixedIntersection.Count, GetAllInvolvedParties(smallerQS).Parties.Count);

                //now add the fixed part to the threshold
                foreach (var th in threshold)
                {
                    th.fixedParties = fixedIntersection;
                }
                return threshold;
            }
            return null;
        }

        private static List<ThresholdSubset> findThreshold(IEnumerable<QualifiedSubset> candidateSets,int depth,int allparties)
        {
            int allsentences = candidateSets.Count();
            // normal case
            List<ThresholdSubset> thresholds = new List<ThresholdSubset>();
            var ncr = nCr(allparties,depth);

            if( ncr ==  allsentences) {
                //TODO: check frequencies too
                bool allHaveEqualFrequency = IsAllPartiesFrequencyEqual(candidateSets);
                if (allHaveEqualFrequency)
                {
                    ThresholdSubset th = new ThresholdSubset(allparties, depth, new List<Trustee>(), GetAllInvolvedParties(candidateSets).Parties);
                    thresholds.Add(th);
                }
            }
                /// if the candidate set is larger than nCr then probably some sets don't belong to the threshold 
                /// Here we recusrively rotate parties and select all possible combinations to see which ones are 
                /// building threshold for e.g. p1p2p3 p2p3p4 p1p3p4 p1p2p4 p2p4p5 
            else if (allsentences < ncr)
            {
                //all possible candidate sets must be generated and recursively called
                if (allsentences > 2)
                {
                    var nextPossibleCombination = GetNextPossibleCombiantion(allparties, depth, allsentences);
                    if (nextPossibleCombination == 1) return null;
                    var smallersetcombinations = candidateSets.Combinations(nextPossibleCombination);
                    foreach (var smallerset in smallersetcombinations)
                    {
                        if (smallerset != null)
                        {
                            var newallparties = GetAllInvolvedParties(smallerset).Parties.Count;
                            var foundthresholds = findThreshold(smallerset, depth, newallparties);
                            ///try filtering the fixed sets maybe find a threshold 
                            if (foundthresholds == null || foundthresholds.Count == 0)
                            {
                                foundthresholds = findFixedConcatThreshold(smallerset, depth, newallparties);
                            }
                            if (foundthresholds != null && foundthresholds.Count > 0)
                            {
                                thresholds.AddRange(foundthresholds);
                            }
                        }
                    }

                }
            }

            return thresholds;
        }
        static int GetNextPossibleCombiantion(int allParties, int depth,int allsentences)
        {

            var a = nCr(allParties,depth);
            while(a>= allsentences)
                a = nCr(--allParties, depth);

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
            var firstItem = itemsFrequencies.First();
            bool allHaveEqualFrequency = itemsFrequencies.All(s => s.Item2 == firstItem.Item2);
            return allHaveEqualFrequency;
        }

        static bool IsQualifiedSubset(QualifiedSubset subsetToTest, AccessStructure miniamlAccess)
        {
            foreach (var qualifiedSet in miniamlAccess.Accesses)
            {
                HashSet<int> a = new HashSet<int>(subsetToTest.Parties.Select(x => x.GetPartyId()));
                HashSet<int> b = new HashSet<int>(qualifiedSet.Parties.Select(x => x.GetPartyId()));

                if (b.IsProperSubsetOf(a) || b.IsSubsetOf(a)) return true;
            }
            return false;
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
        static List<QualifiedSubset> ExpandPersonPaths(QualifiedSubset qualifiedPersons, List<Trustee> allpersons)
        {
            //TODO: do not expand more than longest subset in the access structure
            if (qualifiedPersons == null) qualifiedPersons = new QualifiedSubset();
            List<QualifiedSubset> result = new List<QualifiedSubset>();
            foreach (Trustee item in allpersons)
            {
                //don't add same party twice
                if (qualifiedPersons.Parties.Contains(item)) continue;
                QualifiedSubset qs = new QualifiedSubset();
                //List<Trustee> ls = new List<Trustee>();
                qs.Parties.AddRange(qualifiedPersons.Parties);
                qs.Parties.Add(item);
                    result.Add(qs);
                //expand the list except current item
                var nextExpansion = allpersons.Where(po=>po.partyId!=item.partyId).ToList();
                //if(!result.Contains(qs))
                result.AddRange(ExpandPersonPaths(qs,nextExpansion ));
            }
            return result;
        }

        static void DumpElements(IEnumerable<QualifiedSubset> sets)
        {
            foreach (var item in sets)
            {
                Console.WriteLine(item);
            }
        }
      
    }
}
