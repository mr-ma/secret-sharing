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
    static class Program
    {
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
            //P1^P2,P2^P3,P3^p4,p4^p5,p5^p6,p6^p7,p7^p8,p8^p1
            AccessStructure access = new AccessStructure("P1^P3^P4,P1^P2,P2^P3,P2^P4");
            List<Trustee> trustees = access.GetAllParties().OrderBy(po=>po.partyId).ToList();

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = ExpandPersonPaths(null, trustees).Distinct().ToList();

            DumpElements(subsets);
           
            //return;

            //we don't care about longer paths which are not mentioned in the access structure
            int longestQualifiedSubsetAccepted = GetLongestLength(access);

            //calculate the share of each party in qualified subsets
            int i = 0;
            List<Tuple< Trustee,int>> allsubsetsparties = new List<Tuple< Trustee,int>>();
            List<QualifiedSubset> qualifiedExpandedSubset = new List<QualifiedSubset>();
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
            #region binomial stuff
            //var qualifiedSets = new List<Tuple<QualifiedSubset, List<QualifiedSubset>>>();
            //var binomialCoefficientList = new List<Tuple<QualifiedSubset, QualifiedSubset>>();
            //CalculateIntersectionsForBionomialCoefficients(qualifiedExpandedSubset, binomialCoefficientList, qualifiedSets);

            ////skip combinations bigger than longest qualified subset and duplications
            //var distincedBinomial = binomialCoefficientList
            //    .Where(po => po.Item1.Parties.Count+po.Item2.Parties.Count <= longestQualifiedSubsetAccepted)
            //    .Distinct();

            //foreach (var item in distincedBinomial)
            //{
            //      Console.WriteLine("Intersect:{0}, Elements:{1}", item.Item1.ToString(), item.Item2.ToString());
            //}
            //var grouped = distincedBinomial.GroupBy(po => new { po.Item1 ,po.Item2.Parties.Count}).Select(group => new { Key = group.Key.Item1,Depth=group.Key.Count, Count = group.Count() });
            //foreach (var item in grouped)
            //{
            //    //find all elements of this key
            //    var elements = distincedBinomial.Where(po => po.Item1.Equals( item.Key) && po.Item2.Parties.Count == item.Depth);
            //    var involvedParties = GetAllInvolvedParties(elements.Select(po=>po.Item2));

            //    /// we are not interested in threshold of the m,m
            //    if (involvedParties.Parties.Count== item.Depth){
            //        Console.WriteLine("Fixed:{0}[{1}]", item.Key, involvedParties);
               
            //    }
            //    else if( item.Count == nCr(involvedParties.Parties.Count, item.Depth))
            //    {
            //        var removeQS = qualifiedSets.Where(po => po.Item1.Equals(item.Key)).Select(po => po.Item2);
            //        var removeQSStr = "";
            //        foreach (var rqs in removeQS)
            //        {
            //            if (rqs[0].Parties.Count == item.Depth)
            //            {
            //                //rqs.Item2.Select(po => po.Item1);
            //                removeQSStr += "[" + rqs[0].ToString() + "]";
            //            }
            //            if (rqs[1].Parties.Count == item.Depth)
            //            {
            //                removeQSStr += "[" + rqs[1].ToString() + "]";
            //            }
            //        }
            //        ThresholdSubset threshold = new ThresholdSubset(involvedParties.Parties.Count, item.Depth, item.Key.Parties, involvedParties.Parties);
            //        thresholdsubsets.Add(threshold);
            //        Console.WriteLine("Fixed:{0} Threshold:({1},{2})[{3}]", item.Key, item.Depth, involvedParties.Parties.Count, involvedParties);
            //    }
            //    //Console.WriteLine("Grouped:{0} depth:{1} Count:{2}",item.Key,item.Depth,item.Count);
            //}
            #endregion

            var normalTH = qualifiedExpandedSubset.GroupBy(po => po.Parties.Count).Select(info => new {Depth = info.Key,Count= info.Count()});
            foreach (var th in normalTH)
            {
               var candidateSets = qualifiedExpandedSubset.Where(po => po.Parties.Count == th.Depth);
               
                ///find threshold in sets
               var threshold = findThreshold(candidateSets,th.Depth,trustees.Count);
               if (threshold.Count == 0) 
               {  
                   //maybe it has a fixed item let's try with fix detector
                   threshold = findFixedConcatThreshold(candidateSets, th.Depth, trustees.Count); 
               }

               if (threshold.Count != 0) 
               { 
                   thresholdsubsets.AddRange(threshold);
               }
               
            }


            foreach (ThresholdSubset th in thresholdsubsets)
            {
                Console.WriteLine(th);
                var coveredsets =ThresholdHelper.ExploreAllSubsets( th);
                Console.WriteLine("covered sets:");
                DumpElements(coveredsets);

                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }

            Console.WriteLine("All remaining subsets");
            DumpElements(qualifiedExpandedSubset);

            //thresholdsubsets + qualifiedExpandedSubset is the share;
            Console.ReadLine();
            return;
        }

        private static QualifiedSubset GetAllInvolvedParties(IEnumerable<QualifiedSubset> elements)
        {
           return  elements.Distinct().Aggregate(
                    (current, next) =>
                    {
                        foreach (var inparty in next.Parties)
                        {
                            if (!current.Parties.Contains(inparty))
                            {
                                current.Parties.Add(inparty);
                            }
                        }
                        return current;
                    });
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
                     smallerQS.Add(new QualifiedSubset( qs.Parties.Except(fixedIntersection)));
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
            // normal case
            List<ThresholdSubset> thresholds = new List<ThresholdSubset>();

            if(nCr(allparties,depth) ==candidateSets.Count() ) {
                ThresholdSubset th = new ThresholdSubset(allparties,depth,new List<Trustee>(),GetAllInvolvedParties(candidateSets).Parties );
                thresholds.Add(th);
            }
            return thresholds;
            
            //List<Tuple<int, int>> partyFrequencies = new List<Tuple<int, int>>();
            //foreach (QualifiedSubset qs in candidateSets)
            //{
            //   partyFrequencies.AddRange(  qs.Parties.GroupBy(po => po.partyId).Select(group => new Tuple<int,int>(group.Key,group.Count() )));
            //}


               ///if cnr(allparties,depth) == all subsets in the length full threshold
        }


        static void CalculateIntersectionsForBionomialCoefficients(IEnumerable<QualifiedSubset> qualifiedExpandedSubset, 
            List< Tuple<QualifiedSubset,
                        QualifiedSubset>> binomialCoefficientList ,List<Tuple<QualifiedSubset, List<QualifiedSubset>>> qualifiedSets )
        {
            foreach (var first in qualifiedExpandedSubset)
            {
                foreach (var second in qualifiedExpandedSubset)
                {
                    //if item is the same or depth are different skip it
                    if (first == second || first.Parties.Count != second.Parties.Count) continue;
                    var intersect = first.Parties.Intersect(second.Parties);
                    var nonintersect = first.Parties.Except(second.Parties).Union(second.Parties.Except(first.Parties));
                    var intersectQS = new QualifiedSubset(intersect);
                    var nonintersectQS = new QualifiedSubset(nonintersect);
                    var initialItems = new List<QualifiedSubset>() { first, second };

                    binomialCoefficientList.Add(new
                        Tuple<QualifiedSubset,
                        QualifiedSubset>(intersectQS, nonintersectQS));
                    qualifiedSets.Add(new Tuple<QualifiedSubset, List<QualifiedSubset>>
                        (intersectQS, new List<QualifiedSubset>() { first, second }));
                    //Console.WriteLine("Intersect:{0}, Elements:{1}", DumpElements(intersect), DumpElements(nonintersect));
                }
            }
        }
        static int GetLongestLength(AccessStructure access)
        {
           return access.Accesses.Max(po => po.Parties.Count);
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

        static List<QualifiedSubset> ExpandAllAccessPaths(List<Trustee> persons)
        {
            List<QualifiedSubset> result = new List<QualifiedSubset>();
            //foreach (Trustee item in persons)
            {
                QualifiedSubset ls = new QualifiedSubset();
               // ls.Parties.Add(item);
                result.AddRange(ExpandPersonPaths(ls, persons/*.Where(po => po.partyId != item.partyId).ToList()*/));
            }
            return result;
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

        private static IEnumerable<int> constructSetFromBits(int i)
        {
            for (int n = 0; i != 0; i /= 2, n++)
            {
                if ((i & 1) != 0)
                    yield return n;
            }
        }

        static List<string>  allValues = new List<string>() { "A1", "A2", "A3", "B1", "B2", "C1" };

        private  static IEnumerable<List<string>> produceEnumeration()
        {
            for (int i = 0; i < (1 << allValues.Count); i++)
            {
                yield return
                    constructSetFromBits(i).Select(n => allValues[n]).ToList();
            }
        }

        public static string produceList()
        {
            var a = produceEnumeration().ToList();
            return "";
        }

        static void permutation(int k, string s)
        {
            for (int j = 1; j < s.Length; ++j)
            {
                swap(s, k % (j + 1), j);
                k = k / (j + 1);
            }
        }

        static void swap(string s, int i, int j)
        {
            //
            // Swaps characters in a string. Must copy the characters and reallocate the string.
            //
            char[] array = s.ToCharArray(); // Get characters
            char temp = array[i]; // Get temporary copy of character
            array[i] = array[j]; // Assign element
            array[j] = temp; // Assign element
            Console.WriteLine(array);
        }

      
    }
}
