using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.ProfilerRunner.Models
{
    public class AccessStructure
    {
        public List<QualifiedSubset> Accesses;
        public AccessStructure()
        {
            this.Accesses = new List<QualifiedSubset>();
        }
        public AccessStructure(String minimalPath)
        {
            this.Accesses = new List<QualifiedSubset>();
            try
            {
                string[] qualifiedsubsets = minimalPath.Split(',');
                foreach (var qs in qualifiedsubsets)
                {
                    QualifiedSubset qualifiedssObj = new QualifiedSubset(qs);
                    this.Accesses.Add(qualifiedssObj);
                }
            }
            catch 
            {
                throw new Exception("Invalid access structure example of valid access: 1^2,3^2,2^3^4,2^5^6");
            }
        }


        public List<Trustee> GetAllParties()
        {
            List<Trustee> parties = new List<Trustee>();
            foreach (QualifiedSubset qs in this.Accesses)
            {
                parties.AddRange(qs.Parties);
            }

            return parties.Distinct().ToList();
        }

       public int GetLongestLength()
        {
            return Accesses.Max(po => po.Parties.Count);
        }
       public static bool IsQualifiedSubset(QualifiedSubset subsetToTest, AccessStructure miniamlAccess)
       {
           foreach (var qualifiedSet in miniamlAccess.Accesses)
           {
               HashSet<int> a = new HashSet<int>(subsetToTest.Parties.Select(x => x.GetPartyId()));
               HashSet<int> b = new HashSet<int>(qualifiedSet.Parties.Select(x => x.GetPartyId()));

               if (b.IsProperSubsetOf(a) || b.IsSubsetOf(a)) return true;
           }
           return false;
       }

       public static List<QualifiedSubset> ExpandPersonPaths(QualifiedSubset qualifiedPersons, List<Trustee> allpersons)
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
               var nextExpansion = allpersons.Where(po => po.partyId != item.partyId).ToList();
               //if(!result.Contains(qs))
               result.AddRange(ExpandPersonPaths(qs, nextExpansion));
           }
           return result;
       }

    }
}
