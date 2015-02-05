using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.Algorithms.GeneralizedAccessStructure;
using System.Collections.Generic;
using System.Linq;
namespace SecretSharing.Test
{
    [TestClass]
    public class AccessStrusctureTest
    {
        [TestMethod]
        public void TestDetectThreshold()
        {

            Trustee p1,p2,p3,p4;
            p1 = new Trustee(1);
            p2 = new Trustee(2);
            p3 = new Trustee(3);
            p4 = new Trustee(4);

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

            List<Trustee> trustees = new List<Trustee>() {p1,p2,p3,p4 };
            ExpandAllAccessPaths(trustees);
        }


        void ExpandAllAccessPaths( List<Trustee> persons)
        {
            List<List<Trustee>> result = new List<List<Trustee>>();
            foreach (Trustee item in persons)
            {
                List<Trustee> ls = new List<Trustee>() { item };
               result.AddRange( ExpandPersonPaths(ls, persons));
            }
        }

        List<List<Trustee>> ExpandPersonPaths(List<Trustee> qualifiedPersons, List<Trustee> allpersons)
        {
           List<List<Trustee>> result = new List<List<Trustee>>();
           foreach (Trustee item in allpersons)
           {
               if (qualifiedPersons.Contains(item)) continue;
               List<Trustee> ls = new List<Trustee>();
               ls.AddRange(qualifiedPersons);
               ls.Add(item);
               result.Add(ls);
               result.AddRange( ExpandPersonPaths(ls, allpersons));
           }
           return result;
        }

    }
}
