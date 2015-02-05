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
            //ProfileRunner runner = new ProfileRunner();
            //runner.Run();

            //Benchmark.ShamirAntixBenchmark benchmark = new Benchmark.ShamirAntixBenchmark();
            //var re =  benchmark.BenchmarkMeWithChunkSize();
            //foreach (var item in re)
            //{
            //    Console.WriteLine(item);
            //}
           // benchmark.BenchmarkMeWithChunkSizeFixedParameters();

            Trustee p1, p2, p3, p4, p5;
            p1 = new Trustee(1);
            p2 = new Trustee(2);
            p3 = new Trustee(3);
            p4 = new Trustee(4);
            //p5 = new Trustee(5);

            // qualified subsets for minimal access structure
           
            //P2^P3 P1^P2^P4 P1^P3^P4
            QualifiedSubset qs1, qs2, qs3;
            
            qs1 = new QualifiedSubset();
            qs1.Parties.Add(p2);
            qs1.Parties.Add(p3);

            qs2 = new QualifiedSubset();
            qs2.Parties.Add(p1);
            qs2.Parties.Add(p2);
            qs2.Parties.Add(p4);

            qs3 = new QualifiedSubset();
            qs3.Parties.Add(p1);
            qs3.Parties.Add(p3);
            qs3.Parties.Add(p4);

            AccessStructure access = new AccessStructure();
            access.Accesses.Add(qs1);
            access.Accesses.Add(qs2);
            access.Accesses.Add(qs3);

            //P1^P2^P3 P1^P2^P4 P1^P3^P4
            /*QualifiedSubset qs1, qs2, qs3;
            qs1 = new QualifiedSubset();
            qs1.Parties.Add(p1);
            qs1.Parties.Add(p2);
            qs1.Parties.Add(p3);
            qs1.Parties.Add(p4);

            qs2 = new QualifiedSubset();
            qs2.Parties.Add(p1);
            qs2.Parties.Add(p2);
            qs2.Parties.Add(p4);
            qs2.Parties.Add(p5);

            qs3 = new QualifiedSubset();
            qs3.Parties.Add(p1);
            qs3.Parties.Add(p2);
            qs3.Parties.Add(p3);
            qs3.Parties.Add(p5);

            AccessStructure access = new AccessStructure();
            access.Accesses.Add(qs1);
            access.Accesses.Add(qs2);
            access.Accesses.Add(qs3);*/



            List<Trustee> trustees = new List<Trustee>() { p1, p2, p3, p4 };

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = ExpandAllAccessPaths(trustees).Distinct().ToList();


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

                var re = "";
                foreach (var item in subset.Parties)
                {
                    item.SecretSharePercentage = 1.0 / subset.Parties.Count;
                    re += (re == "" ? "" : ", ") + item.ToString();

                }
                bool isqualified = IsQualifiedSubset(subset, access);
                if (isqualified)
                {
                    //add the subset to expanded qualified
                    qualifiedExpandedSubset.Add(new Tuple<QualifiedSubset, int>(subset, subset.Parties.Count));
                    allsubsetsparties.AddRange(subset.Parties.Select(po => new Tuple<Trustee, int>(po, subset.Parties.Count)));
                    Console.WriteLine("{0}.\t [{1}] q:{2}", i++, re, isqualified);
                }
            }

            //calculate the frequency of the parties in access structures
            var partiesFrequency = allsubsetsparties.GroupBy(info=>new { info.Item1.partyId, info.Item2 }).Select(group=> new {Key = group.Key.partyId,Depth = group.Key.Item2 , Count = group.Count()});
            partiesFrequency = partiesFrequency.OrderBy(po => po.Depth);
            ///party id , depth 
            List<Tuple<int, int>> secretkeepers = new List<Tuple<int, int>>();
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
                   if (thresholdcandidates.Count() > 0)
                   {
                       //parties with same frequency and same depth are threshold candidates,
                        //where k = frequency , n = count of parties in same depth and frequency
                       string candidates = thresholdcandidates.Select(po => po.Key.ToString()).Aggregate((current, next) => current + ", " + next);
                       Console.WriteLine("threshold ({0},{1}) [{2}]", item.Count, thresholdcandidates.Count(), candidates);
                       thresholds.Add(new Tuple<int, int, string, int>(item.Count, thresholdcandidates.Count(), candidates, item.Depth));
                   }
                }
            }
            var depths = partiesFrequency.Select(po => po.Depth).Distinct().ToList();
            foreach (var depth in depths)
            {
                string secretholders="";
                string thresholders = "";
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
                Console.WriteLine("optimized path: {0} ^ {1}", secretholders, thresholders);
            }
            

            foreach (var item in partiesFrequency)
            {
                Console.WriteLine("partyid:{0},depth:{1} freq:{2}", item.Key, item.Depth, item.Count);
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
            foreach (Trustee item in persons)
            {
                QualifiedSubset ls = new QualifiedSubset();
                ls.Parties.Add(item);
                result.AddRange(ExpandPersonPaths(ls, persons));
            }
            return result;
        }

        static List<QualifiedSubset> ExpandPersonPaths(QualifiedSubset qualifiedPersons, List<Trustee> allpersons)
        {
            //TODO: do not expand more than longest subset in the access structure

            List<QualifiedSubset> result = new List<QualifiedSubset>();
            foreach (Trustee item in allpersons)
            {
                if (qualifiedPersons.Parties.Contains(item)) continue;
                QualifiedSubset qs = new QualifiedSubset();
                //List<Trustee> ls = new List<Trustee>();
                qs.Parties.AddRange(qualifiedPersons.Parties);
                qs.Parties.Add(item);
                result.Add(qs);

                result.AddRange(ExpandPersonPaths(qs, allpersons));
            }
            return result;
        }

        /*
              static List<List<Trustee>> ExpandAllAccessPaths(List<Trustee> persons)
        {
            List<List<Trustee>> result = new List<List<Trustee>>();
            foreach (Trustee item in persons)
            {
                List<Trustee> ls = new List<Trustee>() { item };
                result.AddRange(ExpandPersonPaths(ls, persons));
            }
            return result;
        }

        static List<List<Trustee>> ExpandPersonPaths(List<Trustee> qualifiedPersons, List<Trustee> allpersons)
        {
            List<List<Trustee>> result = new List<List<Trustee>>();
            foreach (Trustee item in allpersons)
            {
                if (qualifiedPersons.Contains(item)) continue;
                List<Trustee> ls = new List<Trustee>();
                ls.AddRange(qualifiedPersons);
                ls.Add(item);
                result.Add(ls);
                result.AddRange(ExpandPersonPaths(ls, allpersons));
            }
            return result;
        }
         */


    }
}
