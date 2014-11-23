using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib.Common
{
    public class SimpleRandom:IRandom
    {
        const int Seed = 1238712339;
        public int[] GetRandomArray(int Length,int Min,int Max)
        {
            if (Length <= 0) throw new Exception("A positive greater than zero Length must be provided");
            int[] randoms = new int[Length];
            Random rnd = new Random(Seed);           
            for (int i = 0; i < Length; i++)
            {
                randoms[i] = rnd.Next(Min, Max);
            }

            return randoms;
        }


        public int GetRandomPrimeNumber()
        {
            //TODO: randomize prime generation
            return 3299;
        }
    }
}
