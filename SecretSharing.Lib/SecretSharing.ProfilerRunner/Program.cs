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
    class Program
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
            AccessStructure access = new AccessStructure("P1,P2^P3,P1^P2^P4,P1^P3^P4");
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
            List<Tuple<QualifiedSubset, int>> qualifiedExpandedSubset = new List<Tuple<QualifiedSubset, int>>();
            foreach (QualifiedSubset subset in subsets)
            {
                //delete unqualified subsets based on minimum access structure
                if (subset.Parties.Count > longestQualifiedSubsetAccepted) continue;
                bool isqualified = IsQualifiedSubset(subset, access);
                if (isqualified)
                {
                    //add the subset to expanded qualified
                    qualifiedExpandedSubset.Add(new Tuple<QualifiedSubset, int>(subset, subset.Parties.Count));
                    allsubsetsparties.AddRange(subset.Parties.Select(po => new Tuple<Trustee, int>(po, subset.Parties.Count)));
                    Console.WriteLine("{0}.\t [{1}] q:{2}", i++, subset.ToString(), isqualified);
                }
            }

            var qualifiedSets = new List<Tuple<QualifiedSubset, List<Tuple<QualifiedSubset, int>>>>();
            var binomialCoefficientList = new List<Tuple<QualifiedSubset, QualifiedSubset>>();
            foreach (var first in qualifiedExpandedSubset)
            {
                foreach (var second in qualifiedExpandedSubset)
                {
                    //if item is the same or depth are different skip it
                    if (first == second || first.Item2 != second.Item2) continue;
                    var intersect = first.Item1.Parties.Intersect(second.Item1.Parties);
                    var nonintersect = first.Item1.Parties.Except(second.Item1.Parties).Union(second.Item1.Parties.Except(first.Item1.Parties));
                    var intersectQS = new QualifiedSubset(intersect);
                    var nonintersectQS = new QualifiedSubset(nonintersect);
                    var initialItems = new List<Tuple<QualifiedSubset, int>>() { first, second };

                    binomialCoefficientList.Add(new
                        Tuple<QualifiedSubset,
                        QualifiedSubset>(intersectQS, nonintersectQS));
                    qualifiedSets.Add(new Tuple<QualifiedSubset, List<Tuple<QualifiedSubset, int>>>(intersectQS, new List<Tuple<QualifiedSubset, int>>() { first, second }));
                    //Console.WriteLine("Intersect:{0}, Elements:{1}", DumpElements(intersect), DumpElements(nonintersect));
                }
            }

            //skip combinations bigger than longest qualified subset and duplications
            var distincedBinomial = binomialCoefficientList
                .Where(po => po.Item1.Parties.Count+po.Item2.Parties.Count <= longestQualifiedSubsetAccepted)
                .Distinct();

            foreach (var item in distincedBinomial)
            {
                  Console.WriteLine("Intersect:{0}, Elements:{1}", item.Item1.ToString(), item.Item2.ToString());
            }



            List<ThresholdSubset> thresholdsubsets = new List<ThresholdSubset>();
            var grouped = distincedBinomial.GroupBy(po => new { po.Item1 ,po.Item2.Parties.Count}).Select(group => new { Key = group.Key.Item1,Depth=group.Key.Count, Count = group.Count() });
            foreach (var item in grouped)
            {
                //find all elements of this key
                var elements = distincedBinomial.Where(po => po.Item1.Equals( item.Key) && po.Item2.Parties.Count == item.Depth);
                var involvedParties = elements.Select(po => po.Item2).Distinct().Aggregate(
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

                /// we are not interested in threshold of the m,m
                if (involvedParties.Parties.Count== item.Depth){
                    Console.WriteLine("Fixed:{0}[{1}]", item.Key, involvedParties);
               
                }
                else if( item.Count == nCr(involvedParties.Parties.Count, item.Depth))
                {
                    var removeQS = qualifiedSets.Where(po => po.Item1.Equals(item.Key)).Select(po => po.Item2);
                    var removeQSStr = "";
                    foreach (var rqs in removeQS)
                    {
                        if (rqs[0].Item1.Parties.Count == item.Depth)
                        {
                            //rqs.Item2.Select(po => po.Item1);
                            removeQSStr += "[" + rqs[0].Item1.ToString() + "]";
                        }
                        if (rqs[1].Item1.Parties.Count == item.Depth)
                        {
                            removeQSStr += "[" + rqs[1].Item1.ToString() + "]";
                        }
                    }
                    ThresholdSubset threshold = new ThresholdSubset(involvedParties.Parties.Count, item.Depth, item.Key.Parties, involvedParties.Parties);
                    thresholdsubsets.Add(threshold);
                    Console.WriteLine("Fixed:{0} Threshold:({1},{2})[{3}]", item.Key, item.Depth, involvedParties.Parties.Count, involvedParties);
                }
                //Console.WriteLine("Grouped:{0} depth:{1} Count:{2}",item.Key,item.Depth,item.Count);
            }


            foreach (ThresholdSubset th in thresholdsubsets)
            {
                var coveredsets = th.GetQualifiedSubsets().Select(po => new Tuple<QualifiedSubset, int>(po, po.Parties.Count));
                qualifiedExpandedSubset = qualifiedExpandedSubset.Except(coveredsets).ToList();
            }
            //thresholdsubsets + qualifiedExpandedSubset is the share;
            Console.ReadLine();
            return;





            //calculate the frequency of the parties in access structures
            var partiesFrequency = allsubsetsparties.GroupBy(info=>new { info.Item1.partyId, info.Item2 }).Select(group=> new {Key = group.Key.partyId,Depth = group.Key.Item2 , Count = group.Count()});
            partiesFrequency = partiesFrequency.OrderBy(po => po.Depth);

            //dump out parties frequency groupped by
            foreach (var item in partiesFrequency)
            {
                Console.WriteLine("partyid:{0},depth:{1} freq:{2}", item.Key, item.Depth, item.Count);
            }
            

            ///party id , depth 
            List<Tuple<int, int>> secretkeepers = new List<Tuple<int, int>>();
            
            ///party id , depth 
            List<Tuple<int, int>> noOptimisableParties = new List<Tuple<int, int>>();

            ///thresholds k,n,parties, depth
            List<Tuple<int, int, string,int>> thresholds = new List<Tuple<int, int,string,int>>();
            foreach (var item in partiesFrequency)
            {
                //if party frequency is eqaual to qualified expanded subset in the depth (in all subsets) the party is a secret keeper/divider
                if (item.Count == qualifiedExpandedSubset.Where(po=>po.Item2==item.Depth).Count())
                {
                    Console.WriteLine("secret keeper partyid:{0}",item.Key);
                    secretkeepers.Add(new Tuple<int, int>(item.Key, item.Depth));
                }
                else
                {
                 
                    var thresholdcandidates = partiesFrequency.Where(po => po.Depth == item.Depth && po.Count == item.Count);
                    // To have a threshold, all combination sentences must exist means nCr(n,r) must be equal to all existing subsets in the level
                    int allrequiredsentences = nCr(thresholdcandidates.Count(), item.Count);
                    ///threshold found
                    if (thresholdcandidates.Count() == allrequiredsentences)
                   {
                       //parties with same frequency and same depth are threshold candidates,
                        //where k = frequency , n = count of parties in same depth and frequency
                       string candidates = thresholdcandidates.Select(po => po.Key.ToString()).Aggregate((current, next) => current + ", " + next);
                       Console.WriteLine("threshold ({0},{1}) [{2}]", item.Count, thresholdcandidates.Count(), candidates);
                       thresholds.Add(new Tuple<int, int, string, int>(item.Count, thresholdcandidates.Count(), candidates, item.Depth));
                   }
                        //no threshold just divide secret normally
                    else
                    {
                        noOptimisableParties.Add(new Tuple<int, int>(item.Key, item.Depth));
                        Console.WriteLine("no threshold respecting to party:{0} just divide secret normally",item.Key);
                    }
                }
            }
            var depths = partiesFrequency.Select(po => po.Depth).Distinct().ToList();
            foreach (var depth in depths)
            {
                string secretholders="";
                string thresholders = "";
                string nooptimisedPath = "";
                var a = secretkeepers.Where(po => po.Item2 == depth);
                if (a.Count() > 0)
                {
                    secretholders = a.Select(po => po.Item1.ToString()).Aggregate((current, next) => current + ", " + next);
                }
                var b = thresholds.Where(po => po.Item4 == depth);

                if (b.Count() > 0)
                {
                    thresholders = b.Select(po => string.Format("threshold({0},{1})[{2}]", po.Item1, po.Item2, po.Item3)).Distinct().Aggregate((current, next) => current + ", " + next);
                }
                var c = noOptimisableParties.Where(po => po.Item2 == depth);
                if (c.Count() > 0)
                {
                    nooptimisedPath = c.Select(po => po.Item1.ToString()).Aggregate((current, next) => current + ", " + next);
                }
                Console.WriteLine("depth:{0} optimized path: {1} ^ {2} , no optimisable{3}",depth, secretholders, thresholders,nooptimisedPath);
            }
            

         
            Console.ReadLine();
            //System.IO.File.WriteAllLines("benchmarkresultsNew.txt", re);
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

        static void DumpElements(List<QualifiedSubset> sets)
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
