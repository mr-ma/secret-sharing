using SecretSharing.Lib.Common;
using SecretSharing.Lib.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSharing.FiniteFieldArithmetic;

namespace SecretSharing.Lib.Shamir
{
    public class ShamirSecretSharing:ISecretSharing
    {
        public ShamirSecretSharing(IRandom Random)
        {
            SetRandomAlgorithm(Random);
        }

        IRandom Random;

        public List<IShare> DivideSecret(int Secret, int K, int N)
        {
            if (K <= 0 || N <= 0 || K > N) throw new Exception("Invalid scheme prameters. Accepted scheme N>0 && K>0 && (!)K>N");
            
            // Get a random Prime number
            var P = Random.GetRandomPrimeNumber();

            //Define finite field of size p
            FiniteField field = new FiniteField(P);

            // obtain k - 1 positive smaller than P random numbers
            var a = Random.GetRandomArray(K - 1, 1, P);

            // construct polynomial of degree k-1
            Func<int, FiniteFieldElement> fx = (int x) =>
            {
                
                var y = new FiniteFieldElement(){Value= Secret,Field = field};
                for (int i = 1; i <= K-1; i++)
                {
                    // our random a starts from 0, therefore we need another access a of index i-1
                    y += new FiniteFieldElement() { Field = field, Value = a[i - 1] * (int)Math.Pow(x, i) };
                }
                return y;
            };

            var shares = new List<IShare>();
            // generated shares
            for (int i = 1; i <= N; i++)
            {
                var myShare = new FiniteFieldShare();
                myShare.X = new FiniteFieldElement() { Value = i, Field = field };
                myShare.Y = fx(i);
                shares.Add(myShare);
            }

            return shares;
        }

        public FiniteFieldElement ReconstructSecret(List<IShare> Shares)
        {
            var interpolate = new FiniteLagrange();
            foreach (FiniteFieldShare share in Shares)
            {
                interpolate.add(share.X, share.Y);
            }
            /// f(0) computes the secret
            var secret = interpolate.interpolateX(((FiniteFieldShare)Shares[0]).X.Zero);

            return secret;
        }


        public void SetRandomAlgorithm(IRandom Random)
        {
            this.Random = Random;
        }
    }
}
