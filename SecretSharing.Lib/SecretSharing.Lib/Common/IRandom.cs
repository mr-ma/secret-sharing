using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib.Common
{
   public interface IRandom
    {
        int[] GetRandomArray(int Length,int Min,int Max);
        int GetRandomPrimeNumber();
    }
}
