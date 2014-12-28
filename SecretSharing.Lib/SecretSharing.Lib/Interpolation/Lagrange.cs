using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib.Interpolation
{
    class Lagrange
    {
        public List<int> allX = new List<int>();
        public List<int> allY = new List<int>();

        public void add(int x, int y)
        {
            allX.Add(x);
            allY.Add(y);

        }

        public int interpolateX(int x)
        {
            int answer = 0;
            for (int i = 0; i <= allX.Count - 1; i++)
            {
                int numerator = 1;
                int denominator = 1;
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
