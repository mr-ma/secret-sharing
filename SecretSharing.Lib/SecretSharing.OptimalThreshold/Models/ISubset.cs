using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.OptimalThreshold.Models
{
    public interface ISubset
    {
         int getPartiesCount();
         int getShareBranchesCount();

         int getPartyId(int index);
    }
}
