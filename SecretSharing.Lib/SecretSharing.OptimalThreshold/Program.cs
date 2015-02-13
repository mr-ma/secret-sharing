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
            //p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7
            //P1^P2,P2^P3,P3^p4,p3^p5,p3^p6,p6^p7,p7^p8,p8^p1
            AccessStructure acc = new AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4,p4^p5^p6,p4^p5^p7,p4^p6^p7");
                       var expanded = new List<QualifiedSubset>();
                    var qualified = new List<QualifiedSubset>();
                    var thresholds = new List<ThresholdSubset>();
                    var remaining = new List<QualifiedSubset>();
                    var attempts = new List<String>();
                    ThresholdHelper.ServeThresholdDetection(acc, out expanded, out  qualified, out  thresholds, out attempts, out remaining);
                    //foreach (var att in attempts)
                    //{
                    //    Console.WriteLine(att);
                    //}      
                    Console.WriteLine("all attempts:{0}",attempts.Count);

                    Console.WriteLine("all found thresholds:");
                    foreach (var thre in thresholds)
                    {
                        Console.WriteLine(thre);
                    }
                    Console.WriteLine("all unique thresholds:");
                    foreach (var thre in ThresholdSubset.CheckInclusiveThresholds(thresholds))
                    {
                        Console.WriteLine(thre);
                    }
            Console.Read();
            return;

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
