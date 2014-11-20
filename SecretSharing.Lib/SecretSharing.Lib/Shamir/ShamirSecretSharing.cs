using SecretSharing.Lib.Common;
using SecretSharing.Lib.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // obtain k - 1 positive smaller than P random numbers
            var a = Random.GetRandomArray(K - 1, 1, P);

            // construct polynomial of degree k-1
            Func<int, int> fx = (int x) =>
            {
                var y = Secret;
                for (int i = 1; i <= K-1; i++)
                {
                    // our random a starts from 0, therefore we need another access a of index i-1
                    y += a[i-1] * (int)Math.Pow(x, i);
                }
                return y;
            };

            var shares = new List<IShare>();
            // generated shares
            for (int i = 1; i <= N; i++)
            {
                var myShare = new Share();
                myShare.X = i;
                myShare.Y = fx(i);
                shares.Add(myShare);
            }

            return shares;
        }

        public int ReconstructSecret(List<IShare> Shares)
        {
            var interpolate = new Lagrange();
            foreach (Share share in Shares)
            {
                interpolate.add(share.X, share.Y);
            }
            /// f(0) computes the secret
            var secret = interpolate.interpolateX(0);

            return secret;
        }


        public void SetRandomAlgorithm(IRandom Random)
        {
            this.Random = Random;
        }
    }
}
