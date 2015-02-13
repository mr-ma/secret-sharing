using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.ProfilerRunner.Models
{
    public class Trustee
    {
        public int partyId;
        public Trustee(int id)
        {
            this.partyId = id;
        }
        public Trustee()
        {
        }
        public Trustee(String id)
        {
            try
            {
                this.partyId = Int32.Parse(id.ToLower().Replace("p", ""));
            }
            catch 
            {
                throw new Exception("Invalid party id use only numbers");
            }
        }
       public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Trustee p = (Trustee)(obj);
            return (this.partyId == p.partyId);
        }
        public override int GetHashCode()
        {
            return partyId;
        }

       public int GetPartyId()
        {
            return partyId;
        }



        public override String ToString()
        {
            return "P" + this.partyId.ToString();
        }
    }
}
