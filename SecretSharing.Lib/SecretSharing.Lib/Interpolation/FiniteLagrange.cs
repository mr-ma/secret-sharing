using SecretSharing.FiniteFieldArithmetic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib.Interpolation
{
    class FiniteLagrange
    {

        public FiniteLagrange()
        {

        }
        public List<FiniteFieldElement> allX = new List<FiniteFieldElement>();
        public List<FiniteFieldElement> allY = new List<FiniteFieldElement>();

        public void add(FiniteFieldElement x, FiniteFieldElement y)
        {
            allX.Add(x);
            allY.Add(y);

        }

        public FiniteFieldElement interpolateX(FiniteFieldElement x)
        {
            FiniteFieldElement answer = x.Zero;
            for (int i = 0; i <= allX.Count - 1; i++)
            {
                FiniteFieldElement numerator = x.One;
                FiniteFieldElement denominator = x.One;
                for (int c = 0; c <= allX.Count - 1; c++)
                {
                    if (c != i)
                    {
                        numerator *= x - allX[c];
                        denominator *=allX[i] - allX[c];
                    }
                }
                answer += (allY[i] * (numerator / denominator));
            }
            return answer;
        }
    }
}
