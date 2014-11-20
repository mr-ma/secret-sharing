using SecretSharing.Lib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib
{
    public interface ISecretSharing
    {

        List<IShare> DivideSecret(int Secret, int K, int N);
        int ReconstructSecret(List<IShare> Shares);
        void SetRandomAlgorithm(IRandom Random);
    }
}
