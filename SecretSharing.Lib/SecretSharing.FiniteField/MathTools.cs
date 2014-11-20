using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.FiniteFieldArithmetic
{
    public static class MathTools
    {
        public static bool IsPrime(double n)
        {
            if (n < 2)
                return false;
            double upTo = Math.Sqrt(n);
            for (int i = 2; i <= upTo; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }

        public static int[] Divisors(this double n)
        {
            int[] result = new int[0];

            for (int i = 1; i <= n; i++)
            {
                if (n % i == 0)
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = i;
                }
            }

            return result;
        }
    }
}
