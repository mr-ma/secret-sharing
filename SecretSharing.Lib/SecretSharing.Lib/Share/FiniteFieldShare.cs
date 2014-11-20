using SecretSharing.FiniteFieldArithmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib
{
    public class FiniteFieldShare:IShare
    {
        FiniteFieldElement x;

        public FiniteFieldElement X
        {
            get { return x; }
            set { x = value; }
        }

        FiniteFieldElement y;

        public FiniteFieldElement Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
