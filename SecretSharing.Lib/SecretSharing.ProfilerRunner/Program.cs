using SecretSharing.ProfilerRunner.Models;
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
            //no threshold: P1^P2,P2^P3,P3^p4,p4^p5,p5^p6,p6^p7,p7^p8,p8^p1
            //threshold ruiner: p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7

            AccessStructure access = new AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7");
            List<Trustee> trustees = access.GetAllParties().OrderBy(po=>po.partyId).ToList();

            //discover all possible expansion of the access structures
            List<QualifiedSubset> subsets = AccessStructure.ExpandPersonPaths(null, trustees).Distinct().ToList();
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
                bool isqualified = AccessStructure. IsQualifiedSubset(subset, access);
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
               var threshold = ThresholdSubset. findThreshold(candidateSets,th.Depth,trustees.Count);
               if (threshold == null || threshold.Count == 0) 
               {  
                   //maybe it has a fixed item let's try with fixed item threshold detector
                   threshold = ThresholdSubset. findFixedConcatThreshold(candidateSets, th.Depth, trustees.Count); 
               }

               if ( threshold!=null && threshold.Count != 0) 
               { 
                   thresholdsubsets.AddRange(threshold);
               }
               
            }

            Console.WriteLine("All detected optimal thresholds:");
            foreach (ThresholdSubset th in ThresholdSubset.CheckInclusiveThresholds(thresholdsubsets.Distinct()))
            {
                Console.WriteLine(th);
                var coveredsets = ThresholdSubset.ExploreAllSubsets(th);
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






         static void  DumpElements(IEnumerable<QualifiedSubset> sets)
        {
            foreach (var item in sets)
            {
                Console.WriteLine(item);
            }
        }

      
  

      
     
      
    }
}
