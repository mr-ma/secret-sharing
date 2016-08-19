using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.OptimalThreshold.Models
{

    public class QualifiedSubset :IComparable,ISubset
    {
        public List<Trustee> Parties;
        public QualifiedSubset()
        {
            this.Parties = new List<Trustee>();
        }
        public QualifiedSubset(String subset)
        {
            this.Parties = new List<Trustee>();
            try
            {
                string[] parties = subset.Split('^');
                foreach (var p in parties)
                {
                    Trustee partyObj = new Trustee(p);
                    this.Parties.Add(partyObj);
                }
            }
            catch
            {
                throw new Exception("Invalid Qualified subset example of valid subset: 1^2^3");
            }
        }
       public  QualifiedSubset(IEnumerable<Trustee> qualifiedPath)
        {
            this.Parties = new List<Trustee>();
            this.Parties.AddRange(qualifiedPath);
        }

        public override bool Equals(Object obj)
        { // no "override" here 
            if (obj == null || GetType() != obj.GetType())
                return false;

            QualifiedSubset p = (QualifiedSubset)(obj);
            if (this.Parties.Count == p.Parties.Count)
            {

                foreach (Trustee var in p.Parties)
                {
                    if (!this.Parties.Contains(var))
                        return false;
                }
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        { // no "override" here
            int re = 1;
            foreach (Trustee t in this.Parties)
            {
                re ^= t.GetPartyId();
            }
            return re;
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
        public static string DumpElementsIntoSingleString(IEnumerable<QualifiedSubset> sets)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in sets)
            {
                sb.AppendFormat(" {0} ", item);
            }
            return sb.ToString();
        }

        string strValue = null;
        public override String ToString()
        {
            if (strValue == null)
            {
                IEnumerable<String> stringified = Parties.Select(po => po.ToString());
                if (stringified.Count() > 0)
                {
                    strValue = "(" + stringified.Aggregate((current, next) => current + "∧" + next) + ")";

                }
                else
                {
                    strValue = "";
                }
            }
            return strValue;
        }


        public int getPartiesCount()
        {
            return this.Parties.Count;
        }


        public int getShareBranchesCount()
        {
            return getPartiesCount();
        }


        public int getPartyId(int index)
        {
            return this.Parties[index].GetPartyId();
        }

        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType()) return 0;
            return this.ToString().CompareTo(((QualifiedSubset)obj).ToString());
        }
    }
}
